
module ActionScript
module Parse


class AccessModifier
  def initialize
    @visibility = nil
    @is_static = false
  end

  attr_accessor :visibility, :is_static

  def private?
    @visibility.to_s == "private"
  end
end

class Argument
  def initialize(name)
    @name = name
    @type = nil
  end

  attr_accessor :name, :type
end

class FunctionSignature
  # Array of Argument objects
  attr_accessor :arguments

  # TypeProxy for return type, or nil if none specified
  attr_accessor :return_type

  # nil, for nornal methods; "get" or "set" for implicit-property access
  # methods
  attr_accessor :implicit_property_modifier
end

class ASParser
  def initialize(lexer)
    @lex = lexer
    @handler = nil
  end

  def handler=(handler)
    @handler = handler
  end


  def parse_compilation_unit
    @handler.compilation_unit_start
    parse_imports_and_attributes
    parse_type_definition
    @handler.compilation_unit_end
  end


  def parse_imports_and_attributes
    while true
      if lookahead?(ImportToken)
        parse_import
      elsif lookahead?(LBracketToken)
	eat_attribute
      else
	break
      end
    end
  end

  def parse_import
    expect(ImportToken)
    @handler.import(parse_class_or_package)
    expect(SemicolonToken)
  end

  def parse_class_or_package
    name = []
    name << expect(IdentifierToken)
    while lookahead?(DotToken)
      expect(DotToken)
      if lookahead?(IdentifierToken)
	name << expect(IdentifierToken)
      elsif lookahead?(StarToken)
        name << expect(StarToken)
	break
      else
	err("Expected <identifier> or <star>, but found #{@lex.peek_next.inspect}")
      end
    end
    name
  end

  def parse_type_definition
    if lookahead?(ClassToken) || lookahead?(DynamicToken) || lookahead?(IntrinsicToken)
      type = parse_class_or_intrinsic_definition
    elsif lookahead?(InterfaceToken)
      type = parse_interface_definition
    else
      err("Expected <class>, <interface> or <intrinsic>, but found #{@lex.peek_next.inspect}")
    end
  end

  def parse_class_or_intrinsic_definition
    dynamic = false
    intrinsic = false
    while true
      if lookahead?(DynamicToken)
	expect(DynamicToken)
        dynamic = true
      elsif lookahead?(IntrinsicToken)
	expect(IntrinsicToken)
        intrinsic = true
      elsif lookahead?(ClassToken)
	break
      else
	raise "Expected <dynamic>, <intrinsic> or <class>, found #{@lex.peek_next.inspect}"
      end
    end
    if intrinsic
      parse_intrinsic_class_definition(dynamic)
    else
      parse_class_definition(dynamic)
    end
  end
  
  def parse_interface_definition
    expect(InterfaceToken)
    name = parse_type_name
    super_name = nil
    speculate(ExtendsToken) do
      super_name = parse_type_name
    end
    expect(LBraceToken)
    @handler.start_interface(name, super_name)
    parse_interface_member_list
    expect(RBraceToken)
    @handler.end_interface
  end
  
  def parse_class_definition(dynamic)
    expect(ClassToken)
    name = parse_type_name
    super_name = nil
    speculate(ExtendsToken) do
      super_name = parse_type_name
    end
    interfaces = []
    speculate(ImplementsToken) do
      interfaces << parse_type_name
      while lookahead?(CommaToken)
        expect(CommaToken)
        interfaces << parse_type_name
      end
    end
    expect(LBraceToken)
    @handler.start_class(dynamic, name, super_name, interfaces)
    parse_class_member_list
    expect(RBraceToken)
    @handler.end_class
  end

  def parse_intrinsic_class_definition(dynamic)
    expect(ClassToken)
    name = parse_type_name
    super_name = nil
    speculate(ExtendsToken) do
      super_name = parse_type_name
    end
    interfaces = []
    speculate(ImplementsToken) do
      interfaces << parse_type_name
      while lookahead?(CommaToken)
        expect(CommaToken)
        interfaces << parse_type_name
      end
    end
    expect(LBraceToken)
    @handler.start_intrinsic_class(dynamic, name, super_name, interfaces)
    parse_intrinsic_member_list
    expect(RBraceToken)
    @handler.end_intrinsic_class
  end

  def parse_type_name
    name = []
    name << expect(IdentifierToken)
    while lookahead?(DotToken)
      expect(DotToken)
      name << expect(IdentifierToken)
    end
    return name
  end

  def parse_class_member_list
    until lookahead?(RBraceToken)
      parse_attributes_and_class_member
    end
  end

  def parse_interface_member_list
    until lookahead?(RBraceToken)
      parse_interface_function
    end
  end

  def parse_intrinsic_member_list
    until lookahead?(RBraceToken)
      parse_intrinsic_member
    end
  end

  def parse_attributes_and_class_member
    speculate(SemicolonToken) do
      # skip spurious semicolons in class bodies
      return
    end
    parse_attribute_list
    parse_class_member
  end

  def parse_class_member
    @handler.access_modifier(parse_access_modifier)
    if lookahead?(VarToken)
      parse_member_field
    elsif lookahead?(FunctionToken)
      parse_member_function
    else
      err("Expected <var> or <function> but found #{@lex.peek_next.inspect}")
    end
  end

  def parse_access_modifier
    access = AccessModifier.new
    while true
      if lookahead?(PublicToken)
        access.visibility = expect(PublicToken)
      elsif lookahead?(PrivateToken)
        access.visibility = expect(PrivateToken)
      elsif lookahead?(StaticToken)
	expect(StaticToken)
        access.is_static = true
      else
	break
      end
    end
    return access
  end

  def parse_member_field
    expect(VarToken)
    while true
      name = expect(IdentifierToken)
      type = nil
      if lookahead?(ColonToken)
        type = parse_type_spec
      end
      @handler.start_member_field(name, type)
      speculate(AssignToken) do
        eat_field_initializer_expression
      end
      @handler.end_member_field
      if lookahead?(CommaToken)
        expect(CommaToken)
      else
        break
      end
    end
    expect(SemicolonToken)
  end

  def eat_field_initializer_expression
    # appearence of 'function' will break out of the loop below, but we need
    # to handle fields initialized with function-literals too, so eat any
    # initial 'function' here,
    @lex.get_next if lookahead?(FunctionToken)

    # Don't just halt on ';', as actionscript allows the semicolon to be
    # missing.  We halt on any token that could indicate start of next
    # class member.
    until lookaheads?(SemicolonToken, VarToken, FunctionToken, PublicToken, PrivateToken, StaticToken)
      if lookahead?(LBraceToken)
	eat_block
      else
	@lex.get_next
      end
    end
  end

  def parse_member_function
    expect(FunctionToken)
    name = expect(IdentifierToken)
    implicit_property_modifier = nil
    if lookahead?(IdentifierToken)
      if name.body == "set"
	implicit_property_modifier = "set"
	name = expect(IdentifierToken)
      elsif name.body == "get"
	implicit_property_modifier = "get"
	name = expect(IdentifierToken)
      end
    end
    sig = parse_function_signature
    sig.implicit_property_modifier = implicit_property_modifier
    eat_block do
      @handler.member_function(name, sig)
    end
  end

  def parse_interface_function
    @handler.access_modifier(parse_access_modifier)
    expect(FunctionToken)
    name = expect(IdentifierToken)
    sig = parse_function_signature
    @handler.interface_function(name, sig)
    expect(SemicolonToken)
  end

  def parse_intrinsic_member
    speculate(SemicolonToken) do
      # skip spurious semicolons in class bodies
      return
    end
    @handler.access_modifier(parse_access_modifier)
    if lookahead?(VarToken)
      parse_member_field
    elsif lookahead?(FunctionToken)
      parse_intrinsic_member_function
    else
      err("Expected <var> or <function> but found #{@lex.peek_next.inspect}")
    end
  end

  def parse_intrinsic_member_function
    @handler.access_modifier(parse_access_modifier)
    expect(FunctionToken)
    name = expect(IdentifierToken)
    sig = parse_function_signature
    @handler.intrinsic_member_function(name, sig)
    expect(SemicolonToken)
  end

  def parse_function_signature
    sig = FunctionSignature.new
    expect(LParenToken)
    sig.arguments = parse_formal_argument_list
    expect(RParenToken)
    if lookahead?(ColonToken)
      sig.return_type = parse_type_spec
    end
    return sig
  end

  def parse_type_spec
    expect(ColonToken)
    parse_type_name
  end

  def parse_formal_argument_list
    list = []
    if lookahead?(IdentifierToken)
      list << parse_formal_argument
      while lookahead?(CommaToken)
        expect(CommaToken)
        list << parse_formal_argument
      end
    end
    list
  end

  def eat_block
    open = expect(LBraceToken)
    yield if block_given?
    until lookahead?(RBraceToken)
      if lookahead?(LBraceToken)
        eat_block
      else
        if @lex.get_next.nil?
          raise "end of file looking for closing brace to match line #{open.lineno}"
	end
      end
    end
    expect(RBraceToken)
  end

  def parse_formal_argument
    name = expect(IdentifierToken)
    arg = Argument.new(name)
    if lookahead?(ColonToken)
      arg.type = parse_type_spec
    end
    arg
  end

  def parse_attribute_list
    if lookahead?(LBracketToken)
      @handler.start_attribute_list
      while lookahead?(LBracketToken)
	eat_attribute
      end
      @handler.end_attribute_list
    end
  end

  def eat_attribute
    open = expect(LBracketToken)
    until lookahead?(RBracketToken)
      if lookahead?(LBracketToken)
	# REVISIT: not sure of attribute syntax, but if attributes may contain
	#          brackets, we will need to match them up correctly,
        eat_attribute
      else
	if @lex.get_next.nil?
	  raise "end of file looking for closing bracket to match line #{open.lineno}"
	end
      end
    end
    expect(RBracketToken)
  end

 private
  def expect(kind)
    if kind == SemicolonToken && !lookahead?(kind)
      # Allow expect() to return without raising an error when the token
      # expected-but-missing is a ';'.  ActionScript allows semicolons to
      # be missing in certain circumstances; I've not worked out which these
      # are, but they're probably those where the grammar knows unambiguously
      # that a semicolon is required (thus, this 'hack' makes sense).
      return nil
    end
    tok = @lex.get_next
    unless tok.is_a?(kind)
      err("Expected '#{kind}' but found '#{tok.inspect}'");
    end
    tok
  end

  def lookahead?(kind)
    @lex.peek_next.is_a?(kind)
  end

  def lookaheads?(*kinds)
    tnext = @lex.peek_next
    kinds.each do |kind|
      return true if tnext.is_a?(kind)
    end
    false
  end

  def speculate(kind)
    if lookahead?(kind)
      expect(kind)
      yield
    end
  end

  def err(msg)
    raise msg
  end
end

class ASHandler
  def compilation_unit_start; end
  def compilation_unit_end; end

  def import(name); end

  def comment(text); end
  def whitespace(text); end

  def start_class(dynamic, name, super_name, interfaces); end
  def end_class; end

  def start_intrinsic_class(dynamic, name, super_name, interfaces); end
  def end_intrinsic_class; end

  def start_interface(name, super_name); end
  def end_interface; end

  def access_modifier(modifier); end

  def member_function(name, sig); end
  def intrinsic_member_function(name, sig); end
  def interface_function(name, sig); end

  def start_member_field(name, type); end
  def end_member_field; end

  def start_attribute_list; end
  def end_attribute_list; end
end

end # module Parse
end # module ActionScript
