
require 'parse/aslexer'
require 'parse/parser'
require 'api_model'
require 'doc_comment'
require 'parse/doccomment_lexer'
require 'stringio'

# We used to just define the class again to add this attribute, but I want
# to be compatable with Ruby1.6, which doesn' allow 'class ModName::ClassName'
ActionScript::Parse::ASToken.module_eval("attr_accessor :last_comment")


def simple_parse(input, source)
  as_io = ASIO.new(input)
  lex = ActionScript::Parse::ASLexer.new(as_io)
  lex.source = source
  skip = DocASLexer.new(lex)
  parse = DocASParser.new(skip)
  handler = DocASHandler.new(source)
  parse.handler = handler
  parse.parse_compilation_unit
  handler.defined_type
end


def parse_file(file)
  File.open(File.join(file.prefix, file.suffix)) do |io|
    begin
      is_utf8 = detect_bom?(io)
      type = simple_parse(io, file.suffix)
      type.input_file = file
      type.source_utf8 = is_utf8
      return type
    rescue =>e
      $stderr.puts "#{file.suffix}: #{e.message}\n#{e.backtrace.join("\n")}"
    end
  end
end


# Hacked subclass of SkipASLexer that remembers multiline comment tokens as
# they're bing skipped over, and then pokes them into the next real token
# that comes by
class DocASLexer < ActionScript::Parse::SkipASLexer
  def initialize(io)
    super(io)
    @last_comment= nil
  end

  attr_accessor :last_comment
  protected
  def skip?(tok)
    if tok.instance_of?(ActionScript::Parse::MultiLineCommentToken) &&
       tok.body =~ /^\*/
      @last_comment = tok
    end
    result = super(tok)
    unless result
      if @last_comment
        tok.last_comment = @last_comment
	@last_comment = nil
      end
    end
    result
  end
end


# Take the comment tokens stuffed into 'real' tokens by DocASLexer, and
# pass these to our DocASHandler instance for parts of the grammar where
# they might contain API docs
class DocASParser < ActionScript::Parse::ASParser
  def parse_class_or_intrinsic_definition
    snarf_comment
    super()
  end

  def parse_interface_definition
    snarf_comment
    super()
  end

  def parse_class_member
    snarf_comment
    super()
  end

  def parse_interface_function
    snarf_comment
    super()
  end

  private

  def snarf_comment
    @handler.doc_comment @lex.peek_next.last_comment
  end
end


# Builds a model of the API being processed as ActionScript::Parse::Parser
# recognises pieces of the ActionScript grammar
class DocASHandler < ActionScript::Parse::ASHandler
  def initialize(source)
    @source = source
    parse_conf_build = ConfigBuilder.new
    @method_comment_config = parse_conf_build.build_method_config
    @field_comment_config = parse_conf_build.build_field_config
    @type_comment_config = parse_conf_build.build_type_config
  end

  def compilation_unit_start
    @import_manager = ImportManager.new
    @defined_type = nil
  end

  attr_accessor :defined_type

  def doc_comment(comment)
    @doc_comment = comment
  end

  def import(name)
    @import_manager.add_import(name)
  end

  def start_class(dynamic, name, super_name, interfaces)
    @defined_type = ASClass.new(name)
    @type_resolver = LocalTypeResolver.new(@defined_type)
    if @doc_comment
      @defined_type.comment = parse_comment(@type_comment_config, @doc_comment)
    end
    @defined_type.dynamic = dynamic
    if super_name
      @defined_type.extends = @type_resolver.resolve(super_name)
    end
    if interfaces
      interfaces.each do |interface|
        @defined_type.add_interface(@type_resolver.resolve(interface))
      end
    end
    @defined_type.type_resolver = @type_resolver
    @defined_type.import_manager = @import_manager
  end

  def start_intrinsic_class(dynamic, name, super_name, interfaces)
    start_class(dynamic, name, super_name, interfaces)
    @defined_type.intrinsic = true
  end

  def start_interface(name, super_name)
    @defined_type = ASInterface.new(name)
    @type_resolver = LocalTypeResolver.new(@defined_type)
    if @doc_comment
      @defined_type.comment = parse_comment(@type_comment_config, @doc_comment)
    end
    if super_name
      @defined_type.extends = @type_resolver.resolve(super_name)
    end
    @defined_type.type_resolver = @type_resolver
    @defined_type.import_manager = @import_manager
  end

  def access_modifier(modifier)
    @last_modifier = modifier
  end

  def show_modifier
    visibility  = @last_modifier.visibility
    if visibility.instance_of?(ActionScript::Parse::PublicToken)
      print "public "
    elsif visibility.instance_of?(ActionScript::Parse::PrivateToken)
      print "private "
    end
    if @last_modifier.is_static
      print "static "
    end
  end

  def start_member_field(name, type)
    field = ASExplicitField.new(@defined_type, @last_modifier, name.body)
    unless type.nil?
      field.field_type = @type_resolver.resolve(type)
    end
    if @doc_comment
      field.comment = parse_comment(@field_comment_config, @doc_comment)
    end
    @defined_type.add_field(field)
  end

  def interface_function(name, sig)
    member_function(name, sig)
  end

  def intrinsic_member_function(name, sig)
    member_function(name, sig)
  end

  def member_function(name, sig)
    if sig.implicit_property_modifier.nil?
      real_member_function(name, sig)
    else
      implicit_property_function(name, sig)
    end
  end

  private

  def create_method(name, sig)
    method = ASMethod.new(@defined_type, @last_modifier, name.body)
    if sig.return_type
      method.return_type = @type_resolver.resolve(sig.return_type)
    end
    sig.arguments.each do |arg|
      argument = ASArg.new(arg.name.body)
      if arg.type
        argument.arg_type = @type_resolver.resolve(arg.type)
      end
      method.add_arg(argument)
    end
    if @doc_comment
      method.comment = parse_comment(@method_comment_config, @doc_comment)
    end
    method
  end

  def real_member_function(name, sig)
    method = create_method(name, sig)
    if name.body == @defined_type.unqualified_name
      @defined_type.constructor = method
    else
      @defined_type.add_method(method)
    end
  end

  def implicit_property_function(name, sig)
    field = @defined_type.get_field_called(name.body)
    if field.nil?
      field = ASImplicitField.new(@defined_type, name.body)
      @defined_type.add_field(field)
    end
    func = create_method(name, sig)
    if sig.implicit_property_modifier == "get"
      field.getter_method = func
    elsif sig.implicit_property_modifier == "set"
      field.setter_method = func
    else
      raise "unknown property-modifier #{sig.implicit_property_modifier.inspect}"
    end
  end

  def parse_comment(config, comment_token)
    comment_data = CommentData.new

    input = StringIO.new(comment_token.body)
    input.lineno = comment_token.lineno
    lexer = ActionScript::ParseDoc::DocCommentLexer.new(input)
    lexer.source = @source
    parser = ActionScript::ParseDoc::DocCommentParser.new(lexer)
    handler = OurDocCommentHandler.new(comment_data, config, @type_resolver)
    parser.handler = handler

    parser.parse_comment

    comment_data
  end
end


# The following classes could maybe be split into a different unit from those
# above


# Records the classes and packages imported into a compilation unit
class ImportManager
  # FIXME: 'Manager' code smell!  rename ImportList, or something clearer

  def initialize
    @types = []
    @packages = []
  end

  def add_import(name)
    if name.last.instance_of?(ActionScript::Parse::StarToken)
      name.pop
      add_package_import(name)
    else
      add_type_import(name)
    end
  end

  def add_type_import(name)
    @types << name
  end

  def each_type
    @types.each do |type_name|
      yield type_name
    end
  end

  def add_package_import(name)
    @packages << name
  end

  def each_package
    @packages.each do |package_name|
      yield package_name
    end
  end
end


# A proxy for some type referred to by a particular name within a compilation
# unit.  After we've parsed all compilation units, we'll be able to resolve
# what real type this proxy stands for (i.e. becase we'll have found the
# types pulled into the compilation unit by 'import com.example.*')
class TypeProxy
  # TODO: this should be in api_model.rb

  def initialize(containing_type, name)
    @name = name
    @containing_type = containing_type
    @resolved_type = nil
    @lineno = nil
  end

  attr_accessor :name, :containing_type, :resolved_type, :lineno

  def resolved?
    !@resolved_type.nil?
  end

  def local_name
    # TODO: come up with smarter representations for resolved vs. unresolved
    #       types
    @name
  end

  def qualified?
    @name =~ /\./
  end

  def ==(o)
    name==o.name && containing_type==o.containing_type && resolved_type == o.resolved_type && lineno==o.lineno
  end
end


# Resolves type names to instances of TypeProxy for a particular compilation
# unit (the same name could refer to different types in different compilation
# units).
class LocalTypeResolver
  # TODO: This class actually implements a namespace, so maybe should be
  #       renamed as such.  Also, in as much as the TypeProxy objects this
  #       class supplies *aren't* resolved yet, this class is completely
  #       mis-named.

  def initialize(containing_type)
    @containing_type = containing_type
    @named_types = {}
  end

  def resolve(name, lineno=nil)
    if name.is_a?(Array)
      lineno = name.first.lineno
      name = name.join(".")
    end
    type = @named_types[name]
    if type.nil?
      type = TypeProxy.new(@containing_type, name)
      type.lineno = lineno
      @named_types[name] = type
    end
    type
  end

  def each
    @named_types.each_value do |type|
      yield type
    end
  end
end


# Collects types that are produced by parsing compilation units, building the
# package list as types from different packages are added.
#
# #resolve_types can be used to resolve inter-type references after all the
# ActionScript has been parsed, resolving the real types that the collected
# TypeProxy objects are standing in for.
class GlobalTypeAggregator
  # TODO: this structure sucks; responsibility for type resolution should be
  #       entirely seperate from aggregation, not shoe-horned into this class

  def initialize(classpath)
    @classpath = classpath
    @types = []
    @packages = {}
    @parsed_external_types = {}
  end

  def add_type(type)
    @types << type
    package_name = type.package_name
    package = @packages[package_name]
    if package.nil?
      package = ASPackage.new(package_name)
      @packages[package_name] = package
    end
    type.package = package
    package.add_type(type)
  end

  def each_type
    @types.each do |type|
      yield type
    end
  end

  def types
    @types.dup
  end

  def each_package
    @packages.each_value do |package|
      yield package
    end
  end

  def packages
    @packages.values
  end

  def resolve_types
    # Eeek!...
    qname_map = {}
    qname_map[AS_VOID.qualified_name] = AS_VOID
    @types.each do |type|
      qname_map[type.qualified_name] = type
    end
    @types.each do |type|
      local_namespace = qname_map.dup
      local_namespace[type.unqualified_name] = type
      import_types_into_namespace(type, local_namespace)
      import_packages_into_namespace(type, local_namespace)
      resolver = type.type_resolver
      resolver.each do |type_proxy|
	real_type = local_namespace[type_proxy.local_name]
	unless real_type
	  real_type = maybe_parse_external_definition(type_proxy)
	end
	if real_type
	  type_proxy.resolved_type = real_type
	else
	  $stderr.puts "#{type.input_filename}:#{type_proxy.lineno}: Found no definition of type known locally as #{type_proxy.local_name.inspect}"
	end
      end
    end
  end

  private

  def collect_package_types(package_name)
    # TODO: dump this and use ASPackage instead, now it's available
    @types.each do |type|
      if type.package_name == package_name
	yield type
      end
    end
  end

  def import_types_into_namespace(type, local_namespace)
    importer = type.import_manager
    importer.each_type do |type_name|
      import_type = local_namespace[type_name.join(".")]
      import_type = maybe_parse_external_definition(TypeProxy.new(type, type_name.join('.'))) unless import_type
      if import_type
	local_namespace[type_name.last.body] = import_type
      else
	$stderr.puts "#{type.input_filename}:#{type_name.first.lineno}: Couldn't resolve import of #{type_name.join(".").inspect}"
      end
    end
  end

  def import_packages_into_namespace(type, local_namespace)
    importer = type.import_manager
    importer.each_package do |package_name|
      collect_package_types(package_name.join(".")) do |package_type|
	if local_namespace.has_key?(package_type.unqualified_name)
	  $stderr.puts "#{type.input_filename}: #{package_type.unqualified_name} already refers to #{local_namespace[package_type.unqualified_name].qualified_name}"
	end
	local_namespace[package_type.unqualified_name] = package_type
      end
    end
  end

  def classname_to_filename(qualified_class_name)
    return qualified_class_name.sub(/\./, File::SEPARATOR) + ".as"
  end

  def search_classpath_for(qualified_class_name)
    filename = classname_to_filename(qualified_class_name)

    @classpath.each do |path|
      if FileTest.exist?(File.join(path, filename))
	return SourceFile.new(path, filename)
      end
    end

    nil
  end

  def find_file_matching(type_proxy)
    file_name = search_classpath_for(type_proxy.name)
    return file_name unless file_name.nil?
    return nil if type_proxy.qualified?

    type_proxy.containing_type.import_manager.each_package do |package_name|
      candidate_name = package_name.join(".") + "." + type_proxy.name
      file_name = search_classpath_for(candidate_name)
      return file_name unless file_name.nil?
    end

    nil
  end

  def maybe_parse_external_definition(type_proxy)
    source_file = find_file_matching(type_proxy)
    return nil if source_file.nil?
    astype = @parsed_external_types[source_file.suffix]
    return astype unless astype.nil?
    astype = parse_file(source_file)
    astype.document = false
    @parsed_external_types[source_file.suffix] = astype

    astype
  end
end

# vim:softtabstop=2:shiftwidth=2
