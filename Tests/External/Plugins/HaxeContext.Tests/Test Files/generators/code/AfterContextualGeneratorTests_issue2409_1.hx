﻿package;
@:enum private abstract Issue2409_1(Int) {
	var One = 1;
	var Two = 2;
	public toString() {
		switch(this) {
			case One: 
			case Two:
		}
	}
}