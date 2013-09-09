#ifndef SIMPLELOG_H
#define SIMPLELOG_H

#include <fstream>
#include <QDebug>
#include <QTime>

using namespace std;
ofstream logfile;

void SimpleLogHandler(QtMsgType type, const char *msg)
{
    switch (type) {
        case QtDebugMsg:
            logfile << QTime::currentTime().toString().toAscii().data() << " : " << msg << "\n";
            break;
        case QtCriticalMsg:
            logfile << QTime::currentTime().toString().toAscii().data() << " Critical: " << msg << "\n";
            break;
        case QtWarningMsg:
            logfile << QTime::currentTime().toString().toAscii().data() << " Warning: " << msg << "\n";
            break;
        case QtFatalMsg:
            logfile << QTime::currentTime().toString().toAscii().data() <<  " Fatal: " << msg << "\n";
            abort();
    }
    logfile.flush();
}

#endif // SIMPLELOG_H
