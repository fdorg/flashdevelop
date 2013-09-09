package org.flashdevelop.profiler
{
	import flash.display.LoaderInfo;
	import flash.display.Shape;
	import flash.display.Sprite;
	import flash.display.Stage;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.TimerEvent;
	import flash.system.System;
	import flash.utils.Timer;
	import flash.xml.XMLNode;
	import org.flashdevelop.utils.FlashConnect;
	
	/**
	 * ...
	 * @author Philippe
	 */
	public class Main extends Sprite 
	{
		private var ready:int = 0;
		private var target:String;
		private var sampler:SampleRunner;
		private var update:Timer;
		private var id:int;
		private var tempo:int;
		
		/* SETUP */
		
		public function Main():void 
		{
			addEventListener("allComplete", loadComplete);
			
			if (hasParam("host")) FlashConnect.host = loaderInfo.parameters.host;
			if (hasParam("port")) FlashConnect.port = int(loaderInfo.parameters.port);
			
			if (FlashConnect.initialize()) ++ready;
			else FlashConnect.onConnection = onConnection;
			
			sampler = new SampleRunner();
		}
		
		private function hasParam(name:String):Boolean
		{
			return (name in loaderInfo.parameters) && loaderInfo.parameters[name].length > 0;
		}
		
		private function onConnection():void
		{
			configure();
		}
		
		private function loadComplete(e:Event):void 
		{
			removeEventListener("allComplete", loadComplete);
			
			var info:LoaderInfo = e.target as LoaderInfo;
			target = info.url.replace("|", ":");
			
			configure();
		}
		
		private function configure():void
		{
			if (++ready < 2) return;
			
			sampler.pause();
			
			FlashConnect.trace("[Profiling: " + target + "]");
			FlashConnect.flush();
			
			id = new Date().getTime();
			FlashConnect.onReturnData = onReturn;
			
			update = new Timer(100);
			update.addEventListener(TimerEvent.TIMER, update_timer);
			update.start();
			
			sampler.resume();
		}
		
		
		/* PROFILING */
		
		private function onReturn(status:String):void
		{
			sampler.pause();
			
			var res:XML = XML(status);
			var st:int = int(res.@status);
			if (st == 1) // invalid: stop profiling
			{
				update.stop();
				FlashConnect.onReturnData = null;
				sampler.cleanup();
			}
			else if (st == 4) // run GC
			{
				FlashConnect.trace("[GC]");
				FlashConnect.flush();
				System.gc();
			}
			else if (st == 5) // snapshot
			{
				FlashConnect.trace("[Stacks] " + res.@qname);
				getSnapshot(res.@qname);
			}
			
			sampler.resume();
		}
		
		private function getSnapshot(qname:String):void
		{
			sampler.pause();
			
			var out:Array = [ "stacks/" + qname ];
			sampler.snapshotReport(qname, out);
			
			if (out.length > 1)
			{
				var msgNode:XMLNode = new XMLNode(1, null);
				msgNode.attributes.guid = "ccf2c534-db6b-4c58-b90e-cd0b837e61c4";
				msgNode.attributes.cmd = "notify";
				msgNode.nodeName = "message";
				msgNode.appendChild( new XMLNode(3, out.join("|") ));
				
				FlashConnect.send(msgNode);
				FlashConnect.flush();
			}
			
			sampler.resume();
		}
		
		private function update_timer(e:TimerEvent):void 
		{
			sampler.pause();
			
			sampler.liveObjectsCount();
			
			if (++tempo > 10) 
			{
				tempo = 0;
				
				var out:Array = [ id + "/" + System.totalMemory ];
				if (target) 
				{
					out[0] += "/" + target.split("/").join("\\");
					target = null;
				}
				sampler.outputReport(out);
				
				var msgNode:XMLNode = new XMLNode(1, null);
				msgNode.attributes.guid = "ccf2c534-db6b-4c58-b90e-cd0b837e61c4";
				msgNode.attributes.cmd = "notify";
				msgNode.nodeName = "message";
				msgNode.appendChild( new XMLNode(3, out.join("|") ));
				
				FlashConnect.send(msgNode);
				FlashConnect.flush();
			}
			
			sampler.resume();
		}
		
	}
	
}