/** @define {string} */
var BUILD = "debug";

(function(){

/**
* Main class of the app.
*/
function Main(){}

/**
* Entry point of the app.
*/
Main.main = function()
{
	var main = new Main();
	if (!window.HTMLCanvasElement)
	{
		alert("Your browser does not support HTML5 Canvas.");
		return;
	}
	else main.initialize();
	// entry point
}

/**
* Initializes the basics of the app.
*/
Main.prototype.initialize = function()
{
	/**
	* mainCanvas
	*/
	this.mainCanvas = document.getElementById("mainCanvas");
	/**
	* mainStage
	*/
	this.mainStage = new createjs.Stage(this.mainCanvas);
	this.mainStage.snapToPixelsEnabled = true;
	/*
	* createjs
	*/
	createjs.Ticker.addListener(this);
	createjs.Ticker.useRAF = true;
	createjs.Ticker.setFPS(60);
}

/**
* Updates the stage on Ticker tick.
* @param event The recieved tick event.
*/
Main.prototype.tick = function(event)
{
	this.mainStage.update();
}

/**
* Expose class.
*/
window.Main = Main;

})();
