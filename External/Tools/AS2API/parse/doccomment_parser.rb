
module ActionScript
module ParseDoc



class DocCommentParser

  def initialize(lexer)
    @lex = lexer
    @handler = nil
  end

  def handler=(handler)
    @handler = handler
  end

  # this is the main parser entry-point
  def parse_comment
    return unless @lex.peek_next  # TODO: can be removed once we have EOFToken
    @handler.comment_start(@lex.peek_next.lineno)
    while @lex.peek_next
      parse_line
    end
    @handler.comment_end
  end

  def parse_line
    maybe_skip(StarsToken)
    parse_whitespace
    if lookahead?(ParaAtTagToken)
      @handler.start_paragraph_tag(expect(ParaAtTagToken))
    end
    until lookahead?(EndOfLineToken) || eof?
      if lookahead?(InlineAtTagToken)
	parse_inline_tag
      else
	eat_text_token
      end
    end
    unless eof?
      eat_text_token_of_kind(EndOfLineToken)
    end
  end

  def parse_whitespace
    if lookahead?(WhitespaceToken)
      eat_text_token_of_kind(WhitespaceToken)
    end
  end

  def parse_inline_tag
    tok = expect(InlineAtTagToken)
    @handler.start_inline_tag(tok)
    until lookahead?(RBraceToken)
      err("end of input before closing brace for #{tok.inspect}") if eof?
      if lookahead?(LBraceToken)
	parse_brace_pair
      elsif lookahead?(EndOfLineToken)
	eat_text_token
	maybe_skip(StarsToken)
      else
	eat_text_token
      end
    end
    expect(RBraceToken)
    @handler.end_inline_tag
  end

  def parse_brace_pair
    eat_text_token_of_kind(LBraceToken)
    until lookahead?(RBraceToken) || eof?
      if lookahead?(LBraceToken)
	parse_brace_pair
      elsif lookahead?(EndOfLineToken)
	eat_text_token
	maybe_skip(StarsToken)
      else
	eat_text_token
      end
    end
    eat_text_token_of_kind(RBraceToken)
  end

  # treats the text token, whatever kind it may be, as text without special
  # meaning
  def eat_text_token
    @handler.text(@lex.get_next)
  end

  def eat_text_token_of_kind(kind)
    @handler.text(expect(kind))
  end

 private
  def expect(kind)
    tok = @lex.get_next
    unless tok.is_a?(kind)
      err("Expected '#{kind}' but found '#{tok.inspect}'");
    end
    tok
  end

  def eof?
    @lex.peek_next.nil?
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

  def maybe_skip(kind)
    if lookahead?(kind)
      expect(kind)
    end
  end

  def err(msg)
    raise msg
  end

end


class DocCommentHandler
  def comment_start(lineno); end
  def comment_end; end
  def text(text); end
  def start_paragraph_tag(tag); end
  def start_inline_tag(tag); end
  def end_inline_tag; end
end


end # module Parse
end # module ActionScript
