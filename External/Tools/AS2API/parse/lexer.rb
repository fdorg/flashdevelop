
require 'strscan'

module ActionScript
module Parse

# TODO Create an EOFToken (so that we can report its line number)

class ASToken
  def initialize(body, lineno)
    @body = body
    @lineno = lineno
    @source = nil
  end
  def body
    @body
  end
  def lineno
    @lineno
  end
  def to_s
    @body
  end
  attr_accessor :source
end


class AbstractLexer

  def initialize(io)
    @io = io
    @tokens = Array.new
    @eof = false
    @source = nil
  end

  attr_accessor :source

  def get_next
    nextt
  end

  def peek_next
    check_fill()
    @tokens[0]
  end

  protected

  def nextt
    check_fill()
    @tokens.shift
  end

  def check_fill
    if @tokens.empty? && !@io.eof?
      fill()
    end
  end

  def emit(token)
    token.source = @source
    @tokens << token
  end

  def parse_error(text)
    raise "#{@io.lineno}:no lexigraphic match for text starting '#{text}'"
  end
  def warn(message)
    $stderr.puts(message)
  end
end


# This is a Lexer for the tokens of ActionScript 2.0.
class LexerBuilder
  # This is a naive lexer implementation that considers input line-by-line,
  # with special cases to handle multiline tokens (strings, comments).
  # spacial care must be taken to declaire tokens in the 'correct' order (as
  # the fist match wins), and to cope with keyword/identifier ambiguity
  # (keywords have '\b' regexp-lookahead appended)

  def initialize(token_module)
    @matches = []
    @token_module = token_module
  end

  def make_match(match)
    match.gsub("/", "\\/").gsub("\n", "\\n")
  end

  def add_match(match, lex_meth_sym, tok_class_sym)
    @matches << [make_match(match), lex_meth_sym, tok_class_sym]
  end

  def create_keytoken_class(name)
    the_class = Class.new(ASToken)
    the_class.class_eval <<-EOE
    def initialize(lineno)
      super("#{name}", lineno)
    end
    EOE
    @token_module.const_set("#{name.capitalize}Token".to_sym, the_class)
  end

  def make_simple_token(name, value, match)
    class_name = "#{name}Token"
    the_class = Class.new(ASToken)
    the_class.class_eval <<-EOE
    def initialize(lineno)
      super("#{value}", lineno)
    end
    EOE
    @token_module.const_set(class_name, the_class)

    add_match(match, :lex_simple_token, class_name.to_sym)
  end

  def make_keyword_token(name)
    make_simple_token(name.capitalize, name, "#{name}\\b")
  end

  def make_punctuation_token(name, value)
    make_simple_token(name, value, Regexp.escape(value))
  end

  def build_lexer(target_class)
    text = <<-EOS
      def fill
        line = StringScanner.new(@io.readline)
        until line.eos?
    EOS
    @matches.each_with_index do |token_match, index|
      re, lex_method, tok_class = token_match
      text << "if line.scan(/#{re}/)\n"
      if tok_class
      	text << "  emit(#{lex_method.to_s}(:#{tok_class.to_s}, line, @io))\n"
      else
      	text << "  emit(#{lex_method.to_s}(line, @io))\n"
      end
      text << "  next\n"
      text << "end\n"
    end
    text << <<-EOS
          # no previous regexp matched,
          parse_error(line.rest)
        end
      end
    EOS
    target_class.class_eval(text)
  end

end

end # module Parse
end # module ActionScript
