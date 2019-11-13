package editors;
import ace.AceWrap;
import ace.extern.*;
import ace.*;
import editors.Editor;
import electron.Dialog;
import gml.file.*;
import gml.GmlAPI;
import gml.GmlVersion;
import gml.GmlImports;
import electron.FileWrap;
import electron.FileSystem;
import parsers.*;
import js.RegExp;
import js.html.Element;
import ui.Preferences;
import gmx.*;
import yy.*;
import tools.NativeArray;
import tools.NativeString;
import tools.Dictionary;
import tools.StringBuilder;
import haxe.Json;

/**
 * ...
 * @author YellowAfterlife
 */
class Issue2525_1 extends Editor {
	
	public static var container:Element;
	public var session:AceSession;
	private var modePath:String;
	public var lambdaList:Array<String> = [];
	public var lambdaMap:Dictionary<String> = new Dictionary();
	public var lambdas:Dictionary<GmlExtLambda> = new Dictionary();
	
	public function new(file:GmlFile, modePath:String) {
		super(file);
		this.modePath = modePath;
		element = container;
	}
	
	override public function ready():Void {
		if (GmlAPI.version == GmlVersion.live) {
			GmlSeeker.runSync(file.path, file.code, null, file.kind);
		}
		// todo: this does not seem to cache per-version, but not a performance hit either?
		session = new AceSession(file.code, { path: modePath, version: GmlAPI.version });
		session.setUndoManager(new AceUndoManager());
		// todo: does Mac version of GMS2 use Mac line endings? Probably not
		session.setOption("newLineMode", "windows");
		session.setOption("tabSize", Preferences.current.tabSize);
		Preferences.hookSetOption(session);
		if (modePath == "ace/mode/javascript") {
			session.setOption("useWorker", false);
		}
		session.gmlFile = file;
		session.gmlEdit = this;
	}
	
	override public function stateLoad() {
		if (file.path != null) AceSessionData.restore(this);
	}
	override public function stateSave() {
		AceSessionData.store(this);
	}
	
	override public function focusGain(prev:Editor):Void {
		super.focusGain(prev);
		Main.aceEditor.setSession(session);
	}
	
	static function canImport(file:GmlFile) {
		switch (file.kind) {
			case GmlFileKind.Normal,
				GmlFileKind.GmxObjectEvents, GmlFileKind.YyObjectEvents,
				GmlFileKind.GmxTimelineMoments, GmlFileKind.YyTimelineMoments
			: return file.path != null;
			default: return false;
		}
	}
	
	static function canLambda(file:GmlFile) {
		switch (file.kind) {
			case GmlFileKind.Normal,
				GmlFileKind.GmxObjectEvents, GmlFileKind.YyObjectEvents,
				GmlFileKind.GmxTimelineMoments, GmlFileKind.YyTimelineMoments,
				GmlFileKind.GmxConfigMacros, GmlFileKind.GmxProjectMacros
			: return file.path != null;
			default: return false;
		}
	}
	
	override public function load(data:Dynamic):Void {
		var src:String;
		if (data != null) {
			src = data;
		} else switch (file.kind) {
			case Multifile: src = "";
			case Snippets: src = AceSnippets.getText(file.path);
			default: src = FileWrap.readTextFileSync(file.path);
		}
		var gmx:SfGmx, out:String, errors:String;
		function setError(s:String) {
			file.code = s;
			file.path = null;
			file.kind = Extern;
		}
		switch (file.kind) {
			case Extern: file.code = data != null ? data : "";
			case YyShader: file.code = "";
			case Plain, ExtGML, GLSL, HLSL, JavaScript, Snippets: file.code = src;
			case SearchResults: file.code = data;
			case Normal: {
				src = GmlExtCoroutines.pre(src);
				src = GmlExtLambda.pre(this, src);
				src = GmlExtArgs.pre(src);
				file.code = src;
			};
			case Multifile: {
				if (data != null) file.multidata = data;
				NativeArray.clear(file.extraFiles);
				out = ""; errors = "";
				for (item in file.multidata) {
					if (out != "") out += "\n\n";
					out += "#define " + item.name + "\n";
					var itemCode = FileWrap.readTextFileSync(item.path);
					var itemSubs = GmlMultifile.split(itemCode, item.name);
					if (itemSubs == null) {
						errors += "Can't open " + item.name
							+ " for editing: " + GmlMultifile.errorText + "\n";
					} else switch (itemSubs.length) {
						case 0: { };
						case 1: {
							var subCode = itemSubs[0].code;
							out += NativeString.trimRight(subCode);
						};
						default: errors += "Can't open " + item.name
							+ " for editing because it contains multiple scripts.\n";
					}
					file.extraFiles.push(new GmlFileExtra(item.path));
				}
				if (errors == "") {
					// (too buggy)
					//out = GmlExtArgs.pre(out);
					//out = GmlExtImport.pre(out, path);
					GmlSeeker.runSync(file.path, out, "", file.kind);
					file.code = out;
				} else setError(errors);
			};
			//
			case GmxObjectEvents: {
				gmx = SfGmx.parse(src);
				out = GmxObject.getCode(gmx);
				if (out != null) {
					file.code = out;
				} else setError(GmxObject.errorText);
			};
			case YyObjectEvents: {
				if (data == null) data = Json.parse(src);
				var obj:YyObject = data;
				NativeArray.clear(file.extraFiles);
				file.code = obj.getCode(file.path, file.extraFiles);
			};
			//
			case GmxTimelineMoments: {
				gmx = SfGmx.parse(src);
				out = GmxTimeline.getCode(gmx);
				if (out != null) {
					file.code = out;
				} else setError(GmxObject.errorText);
			};
			case YyTimelineMoments: {
				if (data == null) data = Json.parse(src);
				var tl:YyTimeline = data;
				NativeArray.clear(file.extraFiles);
				file.code = tl.getCode(file.path, file.extraFiles);
			};
			//
			case GmxProjectMacros, GmxConfigMacros: {
				gmx = SfGmx.parse(src);
				var notePath = file.notePath;
				var notes = FileWrap.existsSync(notePath)
					? new GmlReader(FileWrap.readTextFileSync(notePath)) : null;
				file.code = GmxProject.getMacroCode(gmx, notes, file.kind == GmxConfigMacros);
			};
			//
			case YySpriteView: {
				if (data == null) data = Json.parse(src);
			};
		}
		file.syncTime();
		if (file.kind != GmlFileKind.Normal && canLambda(file)) {
			file.code = GmlExtLambda.pre(this, file.code);
		}
		if (canImport(file)) {
			file.code = GmlExtImport.pre(file.code, file.path);
		}
	}
	
	public function postpImport(val:String):{vString {
		var val_preImport = val;
		var path = file.path;
		val = GmlExtImport.post(val, path);
		if (val == null) {
			Main.window.alert(GmlExtImport.errorText);
			return null;
		}
		// if there are imports, check if we should be updating the code
		var data = path != null ? GmlSeekData.map[path] : null;
		if (data != null && data.imports != null || GmlExtImport.post_numImports > 0) {
			var next = GmlExtImport.pre(val, path);
			if (GmlFile.current == file) {
				if (data != null && data.imports != null) {
					GmlImports.currentMap = data.imports;
				} else GmlImports.currentMap = GmlImports.defaultMap;
			}
			if (next != val_preImport) {
				var sd = AceSessionData.get(this);
				var session = session;
				session.doc.setValue(next);
				AceSessionData.set(this, sd);
				Main.window.setTimeout(function() {
					var undoManager = session.getUndoManager();
					if (!Preferences.current.allowImportUndo) {
						session.setUndoManager(undoManager);
						undoManager.reset();
					}
					undoManager.markClean();
					file.changed = false;
				});
			}
		}
		return val;
	}
	
	public function postpNormal(out:String):String {
		inline function error(s:String) {
			Main.window.alert(s);
			return null;
		}
		//
		out = GmlExtArgs.post(out);
		if (out == null) return error("Can't process #args:\n" + GmlExtArgs.errorText);
		//
		if (file.kind != ExtGML && Preferences.current.argsFormat != "") {
			if (GmlExtArgsDoc.proc(file)) {
				out = session.getValue();
				out = GmlExtArgs.post(out);
				Main.window.setTimeout(function() {
					file.markClean();
				});
			}
		}
		//
		if (file.kind != ExtGML) {
			out = GmlExtCoroutines.post(out);
			if (out == null) return error(GmlExtCoroutines.errorText);
		}
		//
		return out;
	}
	
	override public function save():Bool {
		var val = session.getValue();
		var path = file.path;
		file.code = val;
		inline function error(s:String) {
			Main.window.alert(s);
			return false;
		}
		GmlFileBackup.save(file, val);
		//
		if (canImport(file)) {
			val = postpImport(val);
			if (val == null) return false;
		}
		//
		if (canLambda(file)) {
			val = GmlExtLambda.post(this, val);
			if (val == null) return error("Can't process #lambda:\n" + GmlExtLambda.errorText);
		}
		//
		var out:String, src:String, gmx:SfGmx;
		var writeFile:Bool = path != null;
		switch (file.kind) {
			case Extern: out = val;
			case Plain, GLSL, HLSL, JavaScript: out = val;
			case Normal, ExtGML: {
				out = postpNormal(val);
				if (out == null) return false;
			};
			case Multifile: {
				out = val;
				/*out = GmlExtArgs.post(out);
				if (out == null) {
					return error("Can't process macro:\n" + GmlExtArgs.errorText);
				}*/
				//
				writeFile = false;
				var next = GmlMultifile.split(out, "<detached code>");
				var map0 = new Dictionary<String>();
				for (item in file.multidata) map0.set(item.name, item.path);
				var errors = "";
				for (item in next) {
					var itemPath = map0[item.name];
					if (itemPath != null) {
						var itemCode = item.code;
						FileWrap.writeTextFileSync(itemPath, itemCode);
					} else errors += "Can't save script " + item.name
						+ " because it is not among the edited group.\n";
				}
				if (errors != "") error(errors);
			};
			case SearchResults: {
				if (file.searchData == null) return false;
				if (!file.searchData.save(file)) return false;
				file.markClean();
				writeFile = false;
				out = null;
			};
			case GmxObjectEvents: {
				gmx = FileWrap.readGmxFileSync(path);
				if (!GmxObject.setCode(gmx, val)) {
					return error("Can't update GMX:\n" + GmxObject.errorText);
				}
				out = gmx.toGmxString();
			};
			case YyObjectEvents: {
				var obj:YyObject = FileWrap.readJsonFileSync(path);
				if (!obj.setCode(path, val)) {
					return error("Can't update YY:\n" + YyObject.errorText);
				}
				out = NativeString.yyJson(obj);
			};
			case GmxTimelineMoments: {
				gmx = FileWrap.readGmxFileSync(path);
				if (!GmxTimeline.setCode(gmx, val)) {
					return error("Can't update GMX:\n" + GmxTimeline.errorText);
				}
				out = gmx.toGmxString();
			};
			case YyTimelineMoments: {
				var tl:YyTimeline = FileWrap.readJsonFileSync(path);
				if (!tl.setCode(path, val)) {
					return error("Can't update YY:\n" + YyTimeline.errorText);
				}
				out = NativeString.yyJson(tl);
			};
			case GmxProjectMacros, GmxConfigMacros: {
				gmx = FileWrap.readGmxFileSync(path);
				var notes = new StringBuilder();
				GmxProject.setMacroCode(gmx, val, notes, file.kind == GmxConfigMacros);
				var notePath = file.notePath;
				if (notes.length > 0) {
					FileWrap.writeTextFileSync(notePath, notes.toString());
				} else if (FileWrap.existsSync(notePath)) {
					FileWrap.unlinkSync(notePath);
				}
				out = gmx.toGmxString();
			};
			case Snippets: {
				AceSnippets.setText(path, val);
				writeFile = false;
				out = null;
			};
			default: return false;
		}
		//
		if (writeFile) FileWrap.writeTextFileSync(path, out);
		file.savePost(out);
		return true;
	}
	override public function checkChanges():Void {
		#if lwedit
		return;
		#end
		var act = Preferences.current.fileChangeAction;
		if (act == Nothing) return;
		var path = file.path;
		if (file.kind == Snippets) return;
		if (file.kind != Multifile) {
			if (path == null || !haxe.io.Path.isAbsolute(path)) return;
			if (!FileSystem.existsSync(path)) {
				switch (Dialog.showMessageBox({
					title: "File missing: " + file.name,
					message: "The source file is no longer found on disk. "
						+ "What would you like to do?",
					buttons: [
						"Keep editing",
						"Close the file"
					], cancelId: 0,
				})) {
					case 1: file.tabEl.querySelector(".chrome-tab-close").click();
					default: file.path = null;
				}
				return;
			}
		}
		var changed = false;
		if (file.kind != GmlFileKind.Multifile) try {
			var time1 = FileSystem.statSync(path).mtimeMs;
			if (time1 > file.time) {
				file.time = time1;
				changed = true;
			}
		} catch (e:Dynamic) {
			trace("Error checking " + path + ": ", e);
		}
		for (pair in file.extraFiles) try {
			var ppath = pair.path;
			if (!haxe.io.Path.isAbsolute(ppath) || !FileSystem.existsSync(ppath)) continue;
			var time1 = FileSystem.statSync(ppath).mtimeMs;
			if (time1 > pair.time) {
				pair.time = time1;
				changed = true;
			}
		} catch (e:Dynamic) {
			trace("Error checking " + pair.path + ": ", e);
		}
		if (changed) try {
			var prev = file.code;
			file.load();
			//
			var rxr = new RegExp("\\r", "g");
			var check_0 = NativeString.trimRight(prev);
			check_0 = NativeString.replaceExt(check_0, rxr, "");
			var check_1 = NativeString.trimRight(file.code);
			check_1 = NativeString.replaceExt(check_1, rxr, "");
			//
			var dlg:Int = 0;
			if (check_0 == check_1) {
				// OK!
			} else if (!file.changed) {
				if (act != Ask) {
					session.setValue(file.code);
				} else dlg = 1;
			} else dlg = 2;
			//
			if (dlg != 0) {
				//Main.console.log(StringTools.replace(prev, "\r", "\\r"));
				//Main.console.log(StringTools.replace(file.code, "\r", "\\r"));
				function printSize(b:Float) {
					inline function toFixed(f:Float):String {
						return (untyped f.toFixed)(2);
					}
					if (b < 10000) return b + "B";
					b /= 1024;
					if (b < 10000) return toFixed(b) + "KB";
					b /= 1024;
					if (b < 10000) return toFixed(b) + "MB";
					b /= 1024;
					return toFixed(b) + "GB";
				}
				var sz1 = printSize(file.code.length);
				var sz2 = printSize(session.getValue().length);
				var bt = Dialog.showMessageBox({
					title: "File conflict for " + file.name,
					message: 'Source file changed ($sz1) ' + (dlg == 2
						? 'but you have unsaved changes ($sz2)'
						: 'while the current version is $sz2'
					) + '. What would you like to do?',
					buttons: ["Reload file", "Keep current", "Open changes in a new tab"],
					cancelId: 1,
				});
				switch (bt) {
					case 0: session.setValue(file.code);
					case 1: { };
					case 2: {
						var name1 = file.name + " <copy>";
						GmlFile.next = new GmlFile(name1, null, SearchResults, file.code);
						ui.ChromeTabs.addTab(name1);
					};
				}
			}
		} catch (e:Dynamic) {
			trace("Error applying changes: ", e);
		}
	}
}
