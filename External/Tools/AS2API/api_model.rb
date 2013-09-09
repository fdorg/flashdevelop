
# These classes represent the data-model upon which as2api operates.  The
# class names are all prefixed with 'AS' becase many would otherwise clash
# with Ruby's inbuild classes with the same name.


# TODO: The interfaces to these classes still, in places, make direct use of
#       types provided by ActionScript::Parse (i.e. methods expecting or
#       returning subclasses of ASToken).  These classes should be refactored
#       to insulate the documentation-generating subsystem from those details


# Superclass for ASClass and ASInterface, one instance of an ASType subclass
# is created per compilation unit successfully parsed
class ASType
  # give this ASType the given name (an array of IdentifierToken values
  # found by the parser)
  def initialize(name)
    @package_name = name[0, name.length-1].join(".")
    @name = name.last.body
    @source_utf8 = false
    @methods = []
    @constructor = nil
    @extends = nil
    @comment = nil
    @type_resolver = nil
    @import_manager = nil
    @input_file = nil
    @document = true
  end

  attr_accessor :package, :extends, :comment, :source_utf8, :type_resolver, :import_manager, :intrinsic, :constructor

  def input_filename
    @input_file.suffix
  end

  def input_file=(file)
    @input_file = file
    sourcepath_location(File.dirname(file.suffix))
  end

  attr_reader :input_file

  def add_method(method)
    @methods << method
  end

  def each_method
    @methods.each do |meth|
      yield meth
    end
  end

  def methods
    @methods.dup
  end

  def methods?
    !@methods.empty?
  end

  def constructor?
    !@constructor.nil?
  end

  def get_method_called(name)
    each_method do |method|
      return method if method.name == name
    end
    nil
  end

  # The type's name, excluding its package-prefix
  def unqualified_name
    @name
  end

  # ascends the hierarchy of resolved supertypes of this type, passing
  # each ASType to the given block.  Stops when a type does not extend
  # anything, or when the class it extends wasn't resolved.
  def each_ancestor
    parent = @extends
    while !parent.nil? && parent.resolved?
      yield parent.resolved_type
      parent = parent.resolved_type.extends
    end
  end

  def has_ancestor?
    !@extends.nil? && @extends.resolved?
  end

  # The whole type name, including package-prefix
  def qualified_name
    if @package_name == ""
      @name
    else
      "#{@package_name}.#{@name}"
    end
  end

  # The package-prefix on this type's name
  def package_name
    @package_name
  end

  # This exists mostly as a hack to handle types that are declaired without
  # a package-prefix 'class SomeClass {', but shich are located in the
  # directory structure such that a package is implied (and indeed used by
  # Flash when the fileis found).
  # 
  # When a type has no package-prefix, and this method is called on it with
  # an argument "com/foobar", we will 're-package' the type to subsequently
  # become 'com.foobar.SomeClass'
  def sourcepath_location(path)
    path = "" if path == "."
    if @package_name == "" and path != ""
      @package_name = path.gsub("/", ".")
    else
      if @package_name != path.gsub("/", ".")
	$stderr.puts("package #{@package_name.inspect} doesn't match location #{path.inspect}")
      end
    end
  end

  # compare two types based on their qualified names
  def <=>(other)
    cmp = qualified_name.downcase <=> other.qualified_name.downcase
    return cmp unless cmp==0
    qualified_name <=> other.qualified_name
  end

  def document?
    @document
  end

  def document=(is_allowed_in_documentation)
    @document = is_allowed_in_documentation
  end
end

class ASVoidType < ASType
  def initialize
    @name = "Void"
    @package_name = ""
    @document = false
  end
end

AS_VOID = ASVoidType.new

# Classes are types that (just for the perposes of API docs) have fields, and
# implement interfaces
class ASClass < ASType
  def initialize(name)
    @dynamic = false
    @interfaces = []
    @fields = []
    super(name)
  end

  attr_accessor :dynamic

  def implements_interfaces?
    !@interfaces.empty?
  end

  def add_interface(name)
    @interfaces << name
  end

  def each_interface
    @interfaces.each do |name|
      yield name
    end
  end

  # like #each_interface, but then also reports each_interface of each_ancestor
  def each_implemented_interface
    each_interface do |interface|
      yield interface.resolved_type if interface.resolved?
    end
    each_ancestor do |supertype|
      supertype.each_interface do |interface|
	yield interface.resolved_type if interface.resolved?
      end
    end
  end

  def add_field(field)
    @fields << field
  end

  def fields?
    !@fields.empty?
  end

  # returns true if this class, or any superclass has fields
  def inherited_fields?
    return true if fields?
    each_ancestor do |supertype|
      return true if supertype.fields?
    end
    false
  end

  def each_field
    @fields.each do |field|
      yield field
    end
  end

  def fields
    @fields.dup
  end

  def get_field_called(name)
    each_field do |field|
      return field if field.name == name
    end
    nil
  end
end

# ASInterface doesn't add anything to the superclass, it just affirms that
# the API only supported by ASClass will not be available here
class ASInterface < ASType
  def initialize(name)
    super(name)
  end

  def implements_interfaces?
    false
  end

  def fields?
    false
  end
end

# A member in some type
class ASMember
  def initialize(containing_type, access, name)
    @containing_type = containing_type
    @access = access
    @name = name
    @comment = nil
  end

  attr_accessor :containing_type, :access, :name, :comment

  # compares two members based on their names
  def <=>(other)
    cmp = name.downcase <=> other.name.downcase
    return cmp unless cmp==0
    name <=> other.name
  end
end

# A method member, which may appear in an ASClass or ASInterface
class ASMethod < ASMember
  def initialize(containing_type, access, name)
    super(containing_type, access, name)
    @return_type = nil
    @args = []
  end

  attr_accessor :return_type

  def add_arg(arg)
    @args << arg
  end

  def arguments
    @args
  end

  def argument(index)
    @args[index]
  end

  def specified_by
    raise "not applicable to interface methods" unless containing_type.is_a?(ASClass)
    containing_type.each_implemented_interface do |interface|
      spec_method = interface.get_method_called(name)
      return spec_method unless spec_method.nil?
    end
    nil
  end

  def overrides
    containing_type.each_ancestor do |as_class|
      as_method = as_class.get_method_called(name)
      return as_method unless as_method.nil?
    end
  end

  def inherited_comment
    raise "method #{name.inspect} has a comment of its own" unless comment.nil?
    containing_type.each_ancestor do |as_class|
      as_method = as_class.get_method_called(name)
      return as_method unless as_method.nil? || as_method.comment.nil?
    end
    if containing_type.is_a?(ASClass)
      containing_type.each_implemented_interface do |as_interface|
        as_method = as_interface.get_method_called(name)
        return as_method unless as_method.nil? || as_method.comment.nil?
      end
    end
  end
end

# A field member, which may appear in an ASClass, but not an ASInterface
class ASField < ASMember
end

class ASExplicitField < ASField
  def initialize(containing_tyye, access, name)
    super(containing_tyye, access, name)
    @field_type = nil
  end

  attr_accessor :field_type

  def readwrite?; true; end

  def read?; true; end

  def write?; true; end
end

# A field implied by the presence of "get" or "set" methods with this name
class ASImplicitField < ASField
  def initialize(containing_tyye, name)
    super(containing_tyye, nil, name)
    @getter_method = nil
    @setter_method = nil
  end

  attr_accessor :getter_method, :setter_method

  def readwrite?
    !(@getter_method.nil? || @setter_method.nil?)
  end

  def read?
    !@getter_method.nil?
  end

  def write?
    !@setter_method.nil?
  end

  def access
    (@getter_method || @setter_method).access
  end

  def comment
    (@getter_method || @setter_method).comment
  end

  def field_type
    if read?
      return @getter_method.return_type
    else
      unless @setter_method.arguments.empty?
	arg = @setter_method.arguments[0]
	return arg.arg_type
      end
    end
    return nil
  end
end

# A formal function parameter, a list of which appear in an ASMethod
class ASArg
  def initialize(name)
    @name = name
    @arg_type = nil
  end

  attr_accessor :name, :arg_type
end

# A simple aggregation of ASType objects
class ASPackage
  def initialize(name)
    @name = name
    @types = []
  end

  attr_accessor :name
  
  def add_type(type)
    @types << type
  end

  def types
    @types
  end

  def each_type
    @types.each do |type|
      yield type
    end
  end

  def classes
    result = []
    each_type do |type|
      result << type if type.instance_of?(ASClass)
    end
    result
  end

  def interfaces
    result = []
    each_type do |type|
      result << type if type.instance_of?(ASInterface)
    end
    result
  end

  def <=>(other)
    cmp = name.downcase <=> other.name.downcase
    return cmp unless cmp==0
    name <=> other.name
  end
end
