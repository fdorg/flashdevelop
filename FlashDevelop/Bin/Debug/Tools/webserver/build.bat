
:: http://cesanta.com/docs/Embed.shtml

:: ATTENTION!
:: make sure the binary is 32 bits (default with Visual C++ Express)

set FLAGS=-DMONGOOSE_NO_DAV -DMONGOOSE_NO_CGI -DMONGOOSE_NO_AUTH -DMONGOOSE_NO_LOGGING

mkdir obj
cl server.c mongoose.c %FLAGS% /TC /MD /Fo.\obj\

