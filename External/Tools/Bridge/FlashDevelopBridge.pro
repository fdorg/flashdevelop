#-------------------------------------------------
#
# Project created by QtCreator 2010-11-15T11:49:20
#
#-------------------------------------------------

QT += core
QT += gui
QT += network
QT += widgets

TARGET = "FlashDevelop Bridge"
TEMPLATE = app

SOURCES += main.cpp \
    mainwindow.cpp \
    filesystemwatcherex.cpp \
    bridgeserver.cpp \
    bridgethread.cpp

HEADERS += mainwindow.h \
    filesystemwatcherex.h \
    bridgeserver.h \
    bridgethread.h \
    simplelog.h

FORMS += mainwindow.ui

RESOURCES += res.qrc

ICON = Images/AppIcon.icns

QMAKE_INFO_PLIST = Deploy/app.plist
