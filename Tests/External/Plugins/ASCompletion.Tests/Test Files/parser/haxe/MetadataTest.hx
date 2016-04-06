package test.test;

class MetadataTest
{
    private function func():Void
    {
    }

    @:allow(flixel)
    private var test:Int;
    
    private function func2()
    {
    }

    @:allow( flixel )
    private var test2:Int;
    
    private function func3():Void
    {
    }

    @author("FlashDevelop")
    @test
    public function test3(arg:Int):Bool
    {
        
    }
}

@:build(ResourceGenerator.build("resource/strings.json"))
@:build(TemplateBuilder.build('
    <div class="mycomponent"></div>'))
class MetaClass
{

}