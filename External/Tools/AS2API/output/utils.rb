
def ensure_dir(path)
  path_components = path.split(File::SEPARATOR)
  base_path = nil
  if path_components.first == ""
    path_components.shift
    base_path = "/"
  end
  path_components.each do |part|
    if base_path.nil?
      base_path = part
    else
      base_path = File.join(base_path, part)
    end
    unless FileTest.exist?(base_path)
      Dir.mkdir(base_path)
    end
  end
end

def write_file(path, name)
  ensure_dir(path)
  name = File.join(path, name)
  File.open(name, "w") do |io|
    yield io
  end
end


# vim:softtabstop=2:shiftwidth=2
