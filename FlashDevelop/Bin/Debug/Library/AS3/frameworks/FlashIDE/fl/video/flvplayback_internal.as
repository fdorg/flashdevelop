package fl.video
{
	/**
	 * Almost all var, const and function definitions in the fl.video	 * package are not made private or protected or package internal,	 * but are instead put into the flvplayback_internal namespace.	 * (There are SOME privates, mostly variables that are accessible	 * via public get/set methods.) The reasoning behind this is that	 * in the past there have been unforeseen use cases which were	 * only achievable by hacking into the private and protected	 * methods, which was possible with AS2 but completely impossible	 * with AS3. So if a user wants to hack, use namespace	 * flvplayback_internal and hack away!	 * 	 * @private
	 */
	public namespace flvplayback_internal;
}
