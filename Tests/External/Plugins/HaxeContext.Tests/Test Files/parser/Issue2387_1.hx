package;
typedef CourseGeneratorSettings = { 
	totalChunks: Int,
	straightRoads: {
		> CourseGeneratorSetting,
		size: Array<Int>,
	},
	corners: CourseGeneratorSetting,
	jumpObstacles: CourseGeneratorSetting,
	smallRoads: CourseGeneratorSetting,
}

typedef CourseGeneratorSetting = {
	range: Array<Int>,
	availability: Array<Float>,
}