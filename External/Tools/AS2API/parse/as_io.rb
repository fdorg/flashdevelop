
# An IO-like object for reading from ActionScrpt source code.
# It's job is to filter out #include directives, so that these don't need to
# be handled at lexer or parser levels.
class ASIO
  def initialize(io)
    @io = io
  end

  def eof?
    @io.eof?
  end

  def readline
    @io.each_line do |line|
      return line unless handle_directives(line)
    end
  end

  def lineno
    @io.lineno
  end

  private

  def handle_directives(line)
    if line =~ /\s*#include/
      # TODO: Implement #include.  We just ignore, at the moment
      return true
    end
    return false
  end
end
