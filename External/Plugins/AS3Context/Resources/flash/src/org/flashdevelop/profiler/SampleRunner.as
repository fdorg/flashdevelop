package org.flashdevelop.profiler 
{
	import flash.events.TimerEvent;
	import flash.sampler.clearSamples;
	import flash.sampler.getSamples;
	import flash.sampler.NewObjectSample;
	import flash.sampler.pauseSampling;
	import flash.sampler.startSampling;
	import flash.utils.getDefinitionByName;
	import flash.utils.getQualifiedClassName;
	import org.flashdevelop.utils.FlashConnect;
	
	/**
	 * ...
	 * @author Philippe / http://philippe.elsass.me
	 */
	public class SampleRunner
	{
		private var refCache:Object = { };
		private var typeMap:Object = { };
		private var running:Boolean;
		private var ignoreTc:TypeContext = new TypeContext(null);
		private var timerTc:TypeContext = new TypeContext(getQualifiedClassName(TimerEvent));
		
		public function SampleRunner() 
		{
			refCache[String] = ignoreTc;
			refCache[QName] = ignoreTc;
			refCache[TimerEvent] = timerTc;
			
			startSampling();
			running = true;
		}
		
		public function cleanup():void
		{
			pause();
			refCache = null;
			typeMap = null;
		}
		
		public function pause():void
		{
			if (!running) return;
			running = false;
			pauseSampling();
		}
		
		public function resume():void
		{
			if (running) return;
			running = true;
			startSampling();
		}
		
		public function outputReport(out:Array):void
		{
			for each(var tc:TypeContext in refCache)
			{
				var count:int = 0;
				var mem:int = 0;
				for each(var info:SampleInfo in tc.obj) 
				{
					mem += info.size;
					count++;
				}
				if (tc == timerTc) count--; // remove one TimerEvent instance so it doesn't look like a leak
				
				if (count > 0) 
				{
					if (tc.isNew)
					{
						tc.isNew = false;
						out.push(tc.index + "/" + count + "/" + mem + "/" + tc.name);
					}
					out.push(tc.index + "/" + count + "/" + mem);
				}
			}
		}
		
		public function snapshotReport(qname:String, out:Array):void
		{
			var tc:TypeContext = typeMap[qname];
			if (!tc) 
			{
				FlashConnect.trace(qname + " has no instance");
				return;
			}
			
			var cpt:int = 0;
			for each(var info:SampleInfo in tc.obj)
			{
				cpt++;
				if (info.stack)
					out.push(info.stack.join(","));
				else 
					out.push("[no stack]");
			}
		}
		
		public function liveObjectsCount():void
		{
			var samples:Object = getSamples();
			var type:String;
			var tc:TypeContext;
			var id:Number;
			
			for each(var sample:Object in samples) 
			{
				if (sample is NewObjectSample) 
				{
					var nos:NewObjectSample = NewObjectSample(sample);
					if (nos.object == undefined) 
						continue;
					
					id = nos.id;
					tc = refCache[nos.type];
					if (!tc) 
					{
						refCache[nos.type] = tc = new TypeContext(getQualifiedClassName(nos.type));
						typeMap[tc.name] = tc;
					}
					else if (tc == ignoreTc) continue;
					tc.obj[nos.object] = new SampleInfo(nos);
				}
			}
			
			clearSamples();
		}
		
	}

}

import flash.sampler.getSize;
import flash.sampler.NewObjectSample;
import flash.utils.Dictionary;

class TypeContext
{
	static private var tcCount:int = 0;
	
	public var index:int = ++tcCount;
	public var isNew:Boolean = true;
	public var name:String;
	public var obj:Dictionary = new Dictionary(true);
	
	public function TypeContext(qname:String)
	{
		name = qname;
	}
}

class SampleInfo
{
	public var size:int;
	public var stack:Array;
	
	public function SampleInfo(nos:NewObjectSample)
	{
		size = getSize(nos.object);
		stack = nos.stack;
	}
}

