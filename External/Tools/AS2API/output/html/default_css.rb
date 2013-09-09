
def stylesheet(output_dir)
  name = "style.css"

  # avoid overwriting a (possibly modified) existing stylesheet
  return if FileTest.exist?(File.join(output_dir, name))

  write_file(output_dir, name) do |out|
    out.print <<-HERE
body
{
	padding-left: 10px;
	padding-right: 10px;
	font-family: "Lucida Sans Unicode", "Lucida Grande", "Trebuchet MS", Geneva, Arial, sans-serif;
	font-size: 8pt;
}
h1 
{
	font-size: 11pt;
}
h2, caption
{
	padding: 4px;
	padding-left: 8px;
	background-color: #285090;
	font-weight: bold;
	font-size: 8pt;
	color: #ffffff;
}
h3, h4
{
	font-size: 8pt;
}
a 
{
	color: #285090;
}
a:hover 
{
	color: #a30000;
}
.method_details, .field_details
{
	border-top: 1px solid #285090;
	border-top: 1px dotted #285090;
} 
h2 + .method_details, h2 + .field_details
{
	border-top: none;
}
.method_synopsis, .field_synopsis
{
	font-size: 8pt;
}
.type_hierachy ul
{	
	margin-left: 15px;
	margin-bottom: 20px;
	padding-left: 0px;
}
.type_hierachy li
{
	margin-left: 0px;
	padding-left: 0px;
}
.main_nav 
{
	margin-left: 0px;
	padding-left: 0px;
	font-weight: bold;
	font-size: 8pt;
}
.main_nav li 
{
	padding-left: 0px;
	padding-right: 20px;
	display: inline;
}
.main_nav li:before
{
	content: "» ";
}
.main_nav span 
{
	color: #000000;
}
.summary_list
{
	width: 100%;
	border-collapse: collapse;
	margin-bottom: 18px;
}
.summary_list + .footer
{
	margin-top: 0px;
}
.summary_list td
{
	padding: 4px;
	font-size: 8pt;
	border: 1px solid #285090;
	padding-left: 8px;
}
.summary_list td:first-child
{
	width: 30%;
}
.summary_list caption 
{
	padding: 4px;
	text-align: left;
	padding-left: 8px;
	margin-left: -1px;
	background-color: #285090;
	font-weight: bold;
	font-size: 8pt;
}
.navigation_list 
{
	padding-left: 0px;
	margin-left: 0px;
}
.navigation_list li 
{
	margin-bottom: 4px;
	list-style: none;
}
table.arguments td
{
	vertical-align: text-top;
	padding: 0px 15px 0px 0px;
}
.footer
{
	padding-top: 12px;
	border-top: 1px dotted #285090;
	margin-bottom: 18px;
	margin-top: 18px;
}
.code
{
	padding: 4px;
	padding-left: 3px;
	padding-bottom: 6px;
	background-color: #eeeeee;
}
.lineno 
{
	color: #666666;
	border-right: 1px solid #666666;
	margin-right: 5px;
}
.comment 
{ 
	color: #008000; 
}
.comment.doc 
{ 
	color: #008000; 
}
.str_const, .num_const 
{ 
	color: #ff00ff; 
}
.key 
{ 
	font-weight: bolder; 
	color: #000099;
}
    HERE
  end
end

# vim:softtabstop=2:shiftwidth=2
