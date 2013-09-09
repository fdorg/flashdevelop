require 'output/html/html_framework'
require 'output/html/core_pages'
require 'output/html/index'
require 'output/html/sources'
require 'output/html/default_frameset'
require 'output/html/default_css'

def package_list(path_name, type_agregator)
  # REVISIT: Will a package list actually be useful for ActionScript, or can
  #          we always assume that any code that makes reference to a type
  #          must have access to that type's source in order to compile?
  #          (In theory, this file will allow javadoc to link to ActionScript
  #          classes, so maybe keep it just for that.)
  write_file(path_name, "package-list") do |out|
    type_agregator.each_package do |package|
      out.puts(package.name) unless package.name == ""
    end
  end
end

class PageListBuilder
  def initialize(conf, type_agregator)
    @conf = conf
    @type_agregator = type_agregator
  end

  def build_page_list
    list = []
    list << OverviewPage.new(@conf, @type_agregator)
    build_toplevel_frameset_pages(list)
    build_all_package_pages(list)
    build_all_type_pages(list)
    build_all_index_pages(list)
    nav = build_navigation_template
    list.each { |page| page.navigation = nav if page.is_a?(BasicPage) }
    list
  end

  protected

  def build_toplevel_frameset_pages(list)
    list << FramesetPage.new()
    list << OverviewFramePage.new(@type_agregator)
    list << AllTypesFramePage.new(@type_agregator)
  end

  def build_all_package_pages(list)
    last_package = nil
    last_pkg_index = nil
    @type_agregator.each_package do |package|
      pkg_index = PackageIndexPage.new(@conf, package)
      list << pkg_index
      build_package_frameset_pages(list, package)

      if last_package
	pkg_index.prev_package = last_package
	last_pkg_index.next_package = package
      end
      last_package = package
      last_pkg_index = pkg_index
    end
  end

  def build_package_frameset_pages(list, package)
    list << PackageFramePage.new(package)
  end

  def build_all_type_pages(list)
    last_type = nil
    last_type_page = nil
    @type_agregator.each_type do |type|
      if type.document?
	type_page = TypePage.new(@conf, type)
	list << type_page
	list << SourcePage.new(@conf, type) if @conf.sources

	if last_type
	  type_page.prev_type = last_type
	  last_type_page.next_type = type
	end

	last_type = type
	last_type_page = type_page
      end
    end
  end

  def build_all_index_pages(list)
    indexer = Indexer.new
    indexer.create_index(@type_agregator)
    list << IndexPage.new(@conf, indexer)
  end

  def build_navigation_template
    elements = []
    elements << OverviewNavLinkBuilder.new(@conf, "Overview")
    elements << PackageNavLinkBuilder.new(@conf, "Package")
    elements << TypeNavLinkBuilder.new(@conf, "Class")
    elements << SourceNavLinkBuilder.new(@conf, "Source") if @conf.sources
    elements << IndexNavLinkBuilder.new(@conf, "Index")
    elements
  end
end

# main entry point into the documentation generation process
def document_types(conf, type_agregator)
  list = PageListBuilder.new(conf, type_agregator).build_page_list
  create_all_pages(conf, list)
  stylesheet(conf.output_dir)
end

# vim:softtabstop=2:shiftwidth=2
