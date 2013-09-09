class Preloader
{
	// this will cause our initialization code to execute on frame 1 automatically
	private static var initted:Boolean = init();
	private static var instance:Preloader;
	private static var preloaderClass:Function;
	private static var initID:Number;
	private static var simulate:Boolean;
	
	private var lvArray:Array;
	private var intervalID:Number;
	private var simulateLoaded:Number;
	
	// override these to provide your own preloading functionality
	private function onInit() {}
	private function onStatus(status:PreloadStatus) {}
	private function onComplete() {}
	
	public static function register(loaderClass:Function,simulateOnly:Boolean):Boolean
	{
		preloaderClass = loaderClass;
		simulate = simulateOnly;
		return true;
	}
	
	// this is the first code executed in the movie
	private static function init():Boolean
	{
		_root.stop();
		initID = setInterval(kickoff,1);
		return true;
	}
	
	// we have to do this later to allow for a different preloader to register itself
	private static function kickoff()
	{
		clearInterval(initID);
		if (preloaderClass != null)
			instance = new preloaderClass();
		else
			instance = new Preloader();
		instance.onInit();		
	}
	
	function Preloader()
	{
		if (simulate)
		{
			simulateLoaded = 0;
			intervalID = setInterval(checkLoadStatic,100);
			onInit();		
			return;
		}
		
		lvArray = [];
		for (var id:String in _root)
		{
			if (id.substring(0,11) == "__library__")
			{
				if (MovieClipLoader)
				{
					var loaderClip:MovieClip
						= _root.createEmptyMovieClip("sharedlib"+lvArray.length, 100-lvArray.length);
					loaderClip._x = -2000;
					loaderClip._y = -2000;
					
					var loader:MovieClipLoader = new MovieClipLoader();
					loader["loaderClip"] = loaderClip;
					loader["url"] = id.substring(11);
					loader.loadClip(loader["url"],loader["loaderClip"]);				
					lvArray.push(loader);
				}
				else
				{
					var lv:LoadVars = new LoadVars();
					lv.url = id.substring(11);
					lv.load(lv.url);
					lvArray.push(lv);
				}
			}
		}
		
		intervalID = setInterval(checkLoadStatic,100);
		onInit();		
	}
	
	private function checkLoadStatic()
	{
		if (simulate == true)
			instance.simulateCheckLoad();
		else
			instance.checkLoad();
	}
	
	private function simulateCheckLoad()
	{
		var status:PreloadStatus = new PreloadStatus();
		status.bytesLoaded = simulateLoaded;
		status.bytesTotal = 1024768;
		status.modulesLoaded = 0;
		status.modulesTotal = 1;
		onStatus(status);
		simulateLoaded += Math.round(20000 * (1 + Math.random()));
		if (simulateLoaded >= 1024768)
		{
			clearInterval(intervalID);
			_root.play();
			onComplete();
		}
	}
	
	private function checkLoad()
	{
		var numLoaded:Number = 0;
		var loadedBytes:Number = _root.getBytesLoaded();
		var totalBytes:Number = _root.getBytesTotal();
		
		// is the main movie loaded?
		if (loadedBytes == totalBytes && totalBytes > 0)
			numLoaded++;
		
		for (var i:String in lvArray)
		{
			if (MovieClipLoader)
			{
				var lv:MovieClipLoader = lvArray[i];
				var progress:Object = lv.getProgress(lv["loaderClip"]);
				loadedBytes += progress.bytesLoaded;
				totalBytes += progress.bytesTotal;
				
				if (loadedBytes == totalBytes && totalBytes > 0)
					numLoaded++;
			}
			else
			{
				var lv:LoadVars = lvArray[i];
				if (lv.loaded) numLoaded++;
				loadedBytes += lv.getBytesLoaded();
				totalBytes += lv.getBytesTotal();
			}
		}
		
		if (!isNaN(loadedBytes) && !isNaN(totalBytes))
		{
			var status:PreloadStatus = new PreloadStatus();
			status.bytesLoaded = loadedBytes;
			status.bytesTotal = totalBytes;
			status.modulesLoaded = numLoaded;
			status.modulesTotal = lvArray.length + 1; // add self
			onStatus(status);
		}
		
		// are we done?
		if (numLoaded == lvArray.length + 1)
		{
			lvArray = null;
			clearInterval(intervalID);
			_root.play();
			onComplete();
		}
	}
	
}
