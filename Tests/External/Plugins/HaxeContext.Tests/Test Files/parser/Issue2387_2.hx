package;
typedef CourseGeneratorSettings = { 
	totalChunks: Int,
	straightRoads: {
		> CourseGeneratorSetting,
		point: {
			x:Int,
			y:Int
		},
		size: Array<Int>,
		foo : {>CourseGeneratorSetting,
			bar: {
				>Bar,
				id:String,
			},
		},
	},
	corners: CourseGeneratorSetting,
	jumpObstacles: CourseGeneratorSetting,
	smallRoads: CourseGeneratorSetting,
}

typedef CourseGeneratorSetting = {
	range: Array<Int>,
	availability: Array<Float>,
}
typedef Bar = {
	field: String
}