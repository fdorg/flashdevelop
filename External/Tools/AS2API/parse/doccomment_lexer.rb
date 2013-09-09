
require 'parse/lexer'

module ActionScript
module ParseDoc

class WhitespaceToken < ActionScript::Parse::ASToken
end

class EndOfLineToken < ActionScript::Parse::ASToken
end

class StarsToken < ActionScript::Parse::ASToken
end

class ParaAtTagToken < ActionScript::Parse::ASToken
  def to_s
    "@#{@body}"
  end
end

class InlineAtTagToken < ActionScript::Parse::ASToken
  def to_s
    "{@#{@body}"
  end
end

class WordToken < ActionScript::Parse::ASToken
end

class DocCommentLexer < ActionScript::Parse::AbstractLexer
  def lex_simple_token(class_sym, match, io)
    ActionScript::ParseDoc.const_get(class_sym).new(io.lineno-1)
  end

  def lex_simplebody_token(class_sym, match, io)
    ActionScript::ParseDoc.const_get(class_sym).new(match[0], io.lineno-1)
  end

  def lex_simplecapture_token(class_sym, match, io)
    ActionScript::ParseDoc.const_get(class_sym).new(match[1], io.lineno-1)
  end
end

END_OF_LINE = "\r\n|\r|\n"
DOC_WHITESPACE = "[ \t\f]"
AT_INLINE_TAG = "\\{@([^ \t\r\n\f}{]+)"
AT_PARA_TAG = "@([^ \t\r\n\f]+)"
WHITESPACE_THEN_STARS = "[ \t]*\\*+"
WORD = "[^ \t\f\n\r}{]+"

def self.build_doc_lexer
  builder = ActionScript::Parse::LexerBuilder.new(ActionScript::ParseDoc)

  builder.add_match(WHITESPACE_THEN_STARS, :lex_simplebody_token, :StarsToken)
  builder.add_match(DOC_WHITESPACE, :lex_simplebody_token, :WhitespaceToken)
  builder.add_match(END_OF_LINE, :lex_simplebody_token, :EndOfLineToken)

  builder.add_match(AT_INLINE_TAG, :lex_simplecapture_token, :InlineAtTagToken)
  builder.add_match(AT_PARA_TAG, :lex_simplecapture_token, :ParaAtTagToken)

  builder.make_punctuation_token(:LBrace, "{")
  builder.make_punctuation_token(:RBrace, "}")

  builder.add_match(WORD, :lex_simplebody_token, :WordToken)

  builder.build_lexer(DocCommentLexer)
end

build_doc_lexer


end
end
