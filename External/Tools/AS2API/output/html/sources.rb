
require 'parse/aslexer'

class SourceNavLinkBuilder < NavLinkBuilder
  def href_on(page)
    if page.astype
      page.astype.unqualified_name+".as.html"
    else
      nil
    end
  end

  def is_current?(page); page.is_a?(SourcePage); end

  def title_on(page)
    if page.astype
      "Source code of #{page.astype.qualified_name}"
    else
      nil
    end
  end
end

class CodeHighlighter

  def initialize
    @number_lines = true
  end

  attr_accessor :number_lines

  Keywords = [
    ActionScript::Parse::AsToken,
    ActionScript::Parse::BreakToken,
    ActionScript::Parse::CaseToken,
    ActionScript::Parse::CatchToken,
    ActionScript::Parse::ClassToken,
    ActionScript::Parse::ConstToken,
    ActionScript::Parse::ContinueToken,
    ActionScript::Parse::DefaultToken,
    ActionScript::Parse::DynamicToken,
    ActionScript::Parse::DeleteToken,
    ActionScript::Parse::DoToken,
    ActionScript::Parse::ElseToken,
    ActionScript::Parse::ExtendsToken,
    ActionScript::Parse::FalseToken,
    ActionScript::Parse::FinallyToken,
    ActionScript::Parse::ForToken,
    ActionScript::Parse::FunctionToken,
    ActionScript::Parse::IfToken,
    ActionScript::Parse::ImplementsToken,
    ActionScript::Parse::ImportToken,
    ActionScript::Parse::InToken,
    ActionScript::Parse::InstanceofToken,
    ActionScript::Parse::InterfaceToken,
    ActionScript::Parse::IntrinsicToken,
    ActionScript::Parse::NewToken,
    ActionScript::Parse::NullToken,
    ActionScript::Parse::PrivateToken,
    ActionScript::Parse::PublicToken,
    ActionScript::Parse::ReturnToken,
    ActionScript::Parse::StaticToken,
    ActionScript::Parse::SuperToken,
    ActionScript::Parse::SwitchToken,
    ActionScript::Parse::ThisToken,
    ActionScript::Parse::ThrowToken,
    ActionScript::Parse::TrueToken,
    ActionScript::Parse::TryToken,
    ActionScript::Parse::TypeofToken,
    ActionScript::Parse::UseToken,
    ActionScript::Parse::VarToken,
    ActionScript::Parse::VoidToken,
    ActionScript::Parse::WhileToken,
    ActionScript::Parse::WithToken
  ]

  class HighlightASLexer < ActionScript::Parse::ASLexer
    def initialize(out, io)
      super(io)
      @lineno = 0
      @out = out
    end

    attr_accessor :number_lines

    def get_next
      tok = super
      out(tok)
      tok
    end

    def out(tok)
      mark_lineno if @number_lines && @lineno == 0
      if Keywords.include?(tok.class)
	pp_tok(tok, "key")
	return
      end
      
      case tok
	when ActionScript::Parse::MultiLineCommentToken
	  if tok.body[0] == "*"[0]
	    pp_tok(tok, "comment doc")
	  else
	    pp_tok(tok, "comment")
	  end
	when ActionScript::Parse::SingleLineCommentToken
	  pp_tok(tok, "comment")
	when ActionScript::Parse::StringToken
	  pp_tok(tok, "str_const")
	when ActionScript::Parse::NumberToken
	  pp_tok(tok, "num_const")
	else
	  p_tok(tok)
      end
    end

    def pp_tok(tok, clazz)
      @out.html_span("class"=>clazz) do
	p_tok(tok)
      end
    end
    def p_tok(tok)
      if @number_lines
	txt = StringScanner.new(tok.to_s)
	until txt.eos?
	  if match = txt.scan_until(/\r\n|\n|\r/)
	    p_str(match)
	    mark_lineno
	  else
	    p_str(txt.rest)
	    txt.terminate
	  end
	end
      else
	p_str(tok.to_s)
      end
    end

    def mark_lineno
      @lineno += 1
      @out.html_span("id"=>@lineno.to_s, "class"=>"lineno") do
	@out.pcdata("%6d  " % [@lineno])
      end
    end

    def p_str(str)
      @out.pcdata(str)
    end
  end

  def highlight(input, output)
    begin
      lex = HighlightASLexer.new(output, input)
      lex.number_lines = @number_lines
      while lex.get_next; end
    rescue => e
      $stderr.puts "#{output.astype.input_filename}:#{e.message}"
    end
  end
end

class SourcePage < BasicPage

  def initialize(conf, type)
    dir = type.package_name.gsub(/\./, "/")
    super(conf, type.unqualified_name+".as", dir)
    @type = type
    @package = type.package
  end

  def generate_body_content
    html_pre do
      file = @type.input_file
      parse(File.join(file.prefix, file.suffix))
    end
  end

  def parse(file)
    File.open(File.join(file)) do |io|
      begin
	is_utf8 = detect_bom?(io)
	as_io = ASIO.new(io)
	highlight = CodeHighlighter.new
	highlight.highlight(as_io, self)
      rescue =>e
	$stderr.puts "#{file}: #{e.message}\n#{e.backtrace.join("\n")}"
      end
    end
  end


  def link_top
    yield "Overview", base_path("overview-summary.html")
  end
end


