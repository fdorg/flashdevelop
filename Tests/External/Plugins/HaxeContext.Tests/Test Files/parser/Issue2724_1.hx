package;
class Issue2724_1 {
	/**
	 *  Get a header
	 *  @param name - Header name to retrieve
	 *  @return `Success(header)` if there is exactly one entry of the given header name, `Failure(err)` otherwise
	 */
	public function byName(name:HeaderName)
	  return switch get(name) {
		 case []:
		Failure(new Error(UnprocessableEntity, headerNotFound(name)));
		 case [v]:
		Success(v);
		 case v: 
		Failure(new Error(UnprocessableEntity, 'Multiple entries for $name header'));
	}
	
    
	/**
	 *  Get the content type header
	 */
	public function contentType() 
	  return byName(CONTENT_TYPE).map(ContentType.ofString);
}