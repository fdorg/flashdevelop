class XMLWriter
  def initialize(io)
    @io = io
  end

  def doctype(name, syspub, public_id, system_id)
    @io.puts("<!DOCTYPE #{name} #{syspub} \"#{public_id}\" \"#{system_id}\">")
  end

  def element(text, attrs=nil)
    start_tag(text, attrs)
    yield
    end_tag(text)
  end

  def simple_element(text, body, attrs=nil)
    start_tag(text, attrs)
    pcdata(body)
    end_tag(text)
  end

  def start_tag(text, attrs=nil)
    chk_name(text)
    @io.print('<')
    @io.print(text)
    attrs(attrs)
    @io.print('>')
  end

  def empty_tag(text, attrs=nil)
    chk_name(text)
    @io.print('<')
    @io.print(text)
    attrs(attrs)
    @io.print('/>')
  end

  def end_tag(text)
    chk_name(text)
    @io.print('</')
    @io.print(text)
    @io.print('>')
  end

  def pcdata(text)
    @io.print(text.gsub(/&/, '&amp;').gsub(/</, '&lt;').gsub(/>/, '&gt;'))
  end

  def cdata(text)
    raise "CDATA text must not contain ']]>'" if text =~ /\]\]>/
    @io.print("<![CDATA[")
    @io.print(text)
    @io.print("]]>")
  end

  def comment(text)
    raise "comment must not contain '--'" if text =~ /--/
    @io.print("<!--")
    @io.print(text)
    @io.print("-->")
  end

  def pi(text)
    raise "processing instruction must not contain '?>'" if text =~ /\?>/
    @io.print("<?")
    @io.print(text)
    @io.print("?>")
  end

  def passthrough(text)
    @io.print(text)
  end

  private
  def chk_name(name)
    raise "bad character '#{$&}' in tag name #{name}" if name =~ /[<>& "']/
  end

  def attrs(attrs)
    unless attrs.nil?
      attrs.each do |key, val|
      	raise "#{key.inspect}=#{val.inspect}" if key.nil? || val.nil?
	@io.print(' ')
	@io.print(key)
	@io.print('="')
	@io.print(val.gsub(/"/, "&quot;"))
	@io.print('"')
      end
    end
  end
end
