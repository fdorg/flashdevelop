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
	main.initialize();
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
	createjs.Ticker.addEventListener("tick", this.mainStage);
	createjs.Ticker.timingMode = createjs.Ticker.RAF;
	createjs.Ticker.setFPS(60);
}

/**
* Expose class.
*/
window.Main = Main;

})();
