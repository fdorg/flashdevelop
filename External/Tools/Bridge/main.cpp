// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include <QApplication>
#include "mainwindow.h"

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    QCoreApplication::setApplicationName("FlashDevelop Bridge");
    QCoreApplication::setOrganizationDomain("www.flashdevelop.org");
    QCoreApplication::setOrganizationName("FlashDevelop.org");
    QCoreApplication::setApplicationVersion("2.0.0");
    MainWindow w;
    //w.show();
    return a.exec();
}
