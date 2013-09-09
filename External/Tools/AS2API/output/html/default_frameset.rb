
require 'output/html/html_framework'


class PackageFramePage < Page

  def initialize(package)
    dir = package_dir_for(package)
    super("package-frame", dir)
    @package = package
    @title = "#{package_description_for(@package)} API Naviation"
    @doctype_id = :transitional
  end

  def generate_content
      html_body do
	html_p do
	  html_a(package_display_name_for(@package), {"href"=>"package-summary.html", "target"=>"type_frame"})
	end
	interfaces = @package.interfaces
	unless interfaces.empty?
	  interfaces.sort!
	  html_h3("Interfaces")
	  html_ul("class"=>"navigation_list") do
	    interfaces.each do |type|
	  
	      html_li do
		link_type(type, false, {"target"=>"type_frame"})
	      end
	    end
	  end
	end
	classes = @package.classes
	unless classes.empty?
	  classes.sort!
	  html_h3("Classes")
	  html_ul("class"=>"navigation_list") do
	    classes.each do |type|
	  
	      html_li do
		link_type(type, false, {"target"=>"type_frame"})
	      end
	    end
	  end
	end
      end
  end

end

class OverviewFramePage < Page

  def initialize(type_agregator)
    super("overview-frame")
    @type_agregator = type_agregator
    @title = "API Overview"
    @doctype_id = :transitional
  end

  def generate_content
      html_body do
	html_h3("Packages")
	html_ul("class"=>"navigation_list") do
	
	  html_li do
	    html_a("(All Types)", {"href"=>"all-types-frame.html", "target"=>"current_package_frame"})
	  end
	  packages = @type_agregator.packages.sort
	  packages.each do |package|
	
	    html_li do
	      name = package_display_name_for(package)
	      
	      html_a(name, {"href"=>package_link_for(package, "package-frame.html"), "target"=>"current_package_frame", "title"=>name})
	    end
	  end
	end
      end
  end

  def extra_metadata
    # this page isn't interesting
    {
      "robots" => "noindex"
    }
  end

end


class AllTypesFramePage < Page

  def initialize(type_agregator)
    super("all-types-frame")
    @type_agregator = type_agregator
    @doctype_id = :transitional
  end

  def generate_content
      html_body do
	html_h3("All Types")
	html_ul("class"=>"navigation_list") do
	  types = @type_agregator.types.sort do |a,b|
	    cmp = a.unqualified_name.downcase <=> b.unqualified_name.downcase
	    if cmp == 0
	      a.qualified_name <=> b.qualified_name
	    else
	      cmp
	    end
	  end
	  types.each do |type|
	    if type.document?
	      href = type.qualified_name.gsub(/\./, "/") + ".html"
	      html_li do
		link_type(type, false, {"target"=>"type_frame"})
	      end
	    end
	  end
	end
      end
  end

  def extra_metadata
    # this page isn't interesting
    {
      "robots" => "noindex"
    }
  end

end


class FramesetPage < Page

  def initialize
    super("index") # HACK - WAS "frameset"
    @doctype_id = :frameset
  end

  def generate_content
    html_frameset("cols"=>"20%,80%") do
      html_frameset("rows"=>"30%,70%") do
	html_frame("src"=>"overview-frame.html",
	                  "name"=>"all_packages_frame",
	                  "title"=>"All Packages")
	html_frame("src"=>"all-types-frame.html",
	                  "name"=>"current_package_frame",
                          "title"=>"All types")
      end
      html_frame("src"=>"overview-summary.html",
                        "name"=>"type_frame",
                        "title"=>"Package and type descriptions")
      html_noframes do
	html_body do
	  html_a("Non-frameset overview page", {"href"=>"overview-summary.html"})
	end
      end
    end
  end

  def extra_metadata
    # this page isn't interesting
    {
      "robots" => "noindex"
    }
  end
end


# vim:softtabstop=2:shiftwidth=2
