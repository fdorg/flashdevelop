/**
* Templates used by the start page.
*/
var rssFeedItemTemplate = "<h3 class=\"rssItemTitle\"><a href=\"javascript:window.external.ShowURL('{1}');\">{0}</a></h3><span class=\"rssItemContent\">{2}</span>";
var projectItemTemplate = "<li onclick=\"javascript:window.external.OpenProject('{1}');\" title=\"{1} ({2})\">{0}</li>";
var tooltipTextTemplate = "{0}<br /><b>" + getLocaleString("tooltipTextType") + "</b> {1}<br /><b>" + getLocaleString("tooltipTextCreated") + "</b> {2}<br /><b>" + getLocaleString("tooltipTextModified") + "</b> {3}";
var versionAvailableTemplate = "<button onclick=\"javascript:document.location.href='{1}';\" class=\"button\"><b>{0}:</b> " + getLocaleString("downloadVersionHere") + "</button>";
var versionOutOfDateTemplate = "<p>" + getLocaleString("yourVersionIsOutOfDate") + "</p>";
var versionNotAvailableTemplate = "<p>" + getLocaleString("versionInfoNotAvailable") + "</p>";

/**
* Parses XML document from string.
*/
function parseXmlDocument(xml)
{
	var parser = new ActiveXObject("Microsoft.XMLDOM");
	parser.async = "false";
	parser.loadXML(xml);
	return parser;
}

/**
* Downloads text document from url.
*/
function loadTextDocument(url, callback)
{
	var loader = new ActiveXObject("Microsoft.XMLHTTP");
	loader.onreadystatechange = function()
	{
		if (loader.readyState == 4)
		{
			callback(loader.responseText, loader.status);
		}
	};
	loader.open("GET", url, true);
	loader.send(null);
}

/**
* Parses the recent project xml document.
*/
function handleProjectXml(xml)
{
	var html = "";
	var projects = new Array();
	var nodes = xml.getElementsByTagName("RecentProject");
	if (nodes.length === 0) html = getLocaleString("recentProjectsNotFound");
	for (var i = 0; i < nodes.length; i++)
	{
		var name = getNodeText(nodes[i].getElementsByTagName("Name"));
		var type = getNodeText(nodes[i].getElementsByTagName("Type"));
		var path = getNodeText(nodes[i].getElementsByTagName("Path")).replace(/\\/g, "\\\\");
		var created = getNodeText(nodes[i].getElementsByTagName("Created"));
		var modified = getNodeText(nodes[i].getElementsByTagName("Modified"));
		var typeDesc = getProjectType(type); // Description of project file type...
		html += formatString(projectItemTemplate, name, addSlashes(path), typeDesc);
	}
	var element = document.getElementById("projectsContent");
	element.innerHTML = "<ul>" + html + "</ul>";
}

function addSlashes(s)
{
	return s.split("'").join("\\'");
}

/**
* Parses the rss feed xml document.
*/
function handleRssFeedXml(text, status)
{
	var html = "";
	var xml = parseXmlDocument(text);
	if (status == 200)
	{
		var items = new Array();
		var xmlItems = xml.getElementsByTagName("item");
		var xmlTitle = getNodeText(xml.getElementsByTagName("title"));
		document.getElementById("rssTitle").innerHTML = xmlTitle;
		for (var i = 0; i < xmlItems.length; i++)
		{
			var title = getNodeText(xmlItems[i].getElementsByTagName("title"));
			var link = getNodeText(xmlItems[i].getElementsByTagName("link")).replace(/\\/g, "\\\\");
			var desc = getNodeText(xmlItems[i].getElementsByTagName("description"));
			html += formatString(rssFeedItemTemplate, title, link, desc);
			//if (i != xmlItems.length - 1) html += "<hr />";
		}
	} 
	else 
	{
		html = getLocaleString("rssFeedNotAvailable");
	}
	var element = document.getElementById("rssContent");
	element.innerHTML = html;
}

/**
* Safe text extraction
*/
function getNodeText(nodes)
{
	if (nodes == null) return ""; //"#ERR#1";
	if (nodes.length == 0) return ""; //"#ERR#2";
	if (nodes[0].firstChild == null) return ""; //"#ERR#3";
	return nodes[0].firstChild.nodeValue;
}

/**
* Handles the downloaded version info.
*/
function handleVersionInfo(text, status)
{
	var html = "";
	if (status == 200)
	{
		var info = text.split(/[\r\n]+/g);
		var version = decodeURIComponent(getUrlParameter("v"));
		html = formatString(versionAvailableTemplate, info[0], info[1]);
		if (version && ((info[0] < version) - (version < info[0])) == -1)
		{
			html += formatString(versionOutOfDateTemplate, version);
		}
	}
	else html = versionNotAvailableTemplate;
	var element = document.getElementById("versionContent");
	element.innerHTML = html;
}

/**
* Gets the localized text for the id.
*/
function getLocaleString(id)
{
	var lang = getUrlParameter("l") || "en_US";
	return locale[lang + "." + id] || id;
}

/**
* Gets the type of the project file.
*/
function getProjectType(extension)
{
	switch (extension)
	{
		case ".fdp" : return getLocaleString("projectTypeFDP");
		case ".as2proj" : return getLocaleString("projectTypeAS2");
		case ".as3proj" : return getLocaleString("projectTypeAS3");
		case ".fdproj" : return getLocaleString("projectTypeGeneric");
		case ".hxproj" : return getLocaleString("projectTypeHaxe");
		case ".lsproj" : return getLocaleString("projectTypeLoom");
		default : return getLocaleString("projectTypeUnknown");
	}
}

/**
* Gets the value of the specified url parameter.
*/
function getUrlParameter(id)
{
	id = id.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
	var regex = new RegExp("[?&]" + id + "=([^&#]*)");
	var results = regex.exec(unescape(window.location.href));
	if (results == null) return "";
	else return results[1];
}

/**
* Formats the string with the specified arguments.
*/
function formatString(text)
{
	var result = text;
	for (var i = 1; i < arguments.length; i++)
	{
		var pattern = "{" + (i - 1) + "}";
		while (result.indexOf(pattern) >= 0)
		{
			result = result.replace(pattern, arguments[i]);
		}
	}
	return result;
}

/**
* Handles the data sent by FlashDevelop.
*/
function handleXmlData(projectXml, rssUrl)
{
	if (rssUrl != null)
	{
		var hdUrl = "https://haxedevelop.org/latest.txt";
		loadTextDocument(hdUrl, handleVersionInfo);
		loadTextDocument(rssUrl, handleRssFeedXml);
	}
	var xml = parseXmlDocument(projectXml);
	handleProjectXml(xml);
}
