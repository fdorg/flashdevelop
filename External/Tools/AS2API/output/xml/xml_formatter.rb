
require 'forwardable'

class XMLFormatter
  extend Forwardable

  def initialize(xml_writer)
    @io = xml_writer
    @indent = "  "
    @level = 0
    @inlines = []
    @indenting = true
  end

  def inlines(elements)
    @inlines = elements
  end

  def_delegators :@io, :doctype, :pcdata, :cdata, :passthrough

  def element(text, attrs=nil)
    old_indent  = @indenting
    indent do
      @indenting = !@inlines.include?(text)
      @io.element(text, attrs) do
        yield
        if @indenting
          @io.passthrough("\n");
          @io.passthrough(@indent * (@level-1));
	end
      end
    end
    @indenting = old_indent
  end

  def simple_element(text, body, attrs=nil)
    indent do
      @io.simple_element(text, body, attrs)
    end
  end

  def empty_tag(text, attrs=nil)
    indent do
      @io.empty_tag(text, attrs)
    end
  end

  def comment(text)
    indent do
      @io.comment(text)
    end
  end

  def pi(text)
    indent do
      @io.pi(text)
    end
  end

  private

  def indent
    if @indenting
      @io.passthrough("\n");
      @io.passthrough(@indent * @level);
      @level += 1
      yield
      @level -= 1
    else
      yield
    end
  end
end

# vim: shiftwidth=2:softtabstop=2
