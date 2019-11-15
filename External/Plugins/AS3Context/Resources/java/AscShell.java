// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
import java.io.*;
import macromedia.asc.util.Context;
import macromedia.asc.util.ContextStatics;
import macromedia.asc.parser.Parser;
//import macromedia.asc.parser.ProgramNode;

/**
 * ASC compiler wrapper
 *
 * Author: Philippe Elsass
 *
 * Building: javac -classpath "C:\path\to\flex_sdk_3\lib\asc.jar;." AscShell.java
 * Running: java -classpath "C:\path\to\flex_sdk_3\lib\asc.jar;." AscShell
 */
class AscShell
{
    static private Context ctx = null;

    // Start a shell waiting for files to parse
    static public void main(String[] args)
    {
        //ContextStatics.useSanityStyleErrors = true;
        ContextStatics statics = new ContextStatics();
        //statics.dialect = 9; // AS3
        //statics.setAbcVersion(1); // FP10
        ctx = new Context(statics);
        //ctx.setLanguage("EN");

        try
        {
            BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
            String rawFile = null;
            StringBuilder src = null;

            while (true)
            {
                String cmd = in.readLine();
                if (cmd == null) break;

                // parse from provided raw file source
                if (cmd.endsWith("$raw$"))
                {
                    if (rawFile != null && src != null)
                    {
                        parseSrc(rawFile, src.toString());
                        rawFile = null;
                        src = null;
                    }
                    else
                    {
                        rawFile = cmd;
                        src = new StringBuilder();
                    }
                }
                else if (src != null)
                {
                    src.append(cmd).append("\n");
                }
                // parse from provided filename
                else parseFile(cmd);
            }
            in.close();
        }
        catch (IOException iex)
        {
            System.out.println(iex);
        }
    }

    // Run Flex SDK Actionscript parser against provided source
    static public void parseSrc(String filespec, String src)
    {
        if (ctx == null) return;

        try
        {
            Parser parser = new Parser(ctx, src, filespec);
            /*ProgramNode pn =*/ parser.parseProgram();
            Thread.sleep(50);
            System.out.println("(ash) Done");
        }
        catch (InterruptedException tex)
        {
            System.out.println("(asc) " + tex.getMessage());
        }
    }

    // Run Flex SDK Actionscript parser against provided file
    static public void parseFile(String filespec)
    {
        if (ctx == null) return;

        try
        {
            BufferedInputStream stream = new BufferedInputStream(new FileInputStream(filespec));
            try
            {
                Parser parser = new Parser(ctx, stream, filespec);
                /*ProgramNode pn =*/ parser.parseProgram();
                Thread.sleep(50);
                System.out.println("(ash) Done");
            }
            catch (InterruptedException tex)
            {
                System.out.println("(asc) " + tex.getMessage());
            }
            finally
            {
                if (stream != null) stream.close();
            }
        }
        catch (IOException iex)
        {
            System.out.println("(ash) " + iex.getMessage());
        }
    }
}
