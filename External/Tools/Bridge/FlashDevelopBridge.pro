#-------------------------------------------------
#
# Project created by QtCreator 2010-11-15T11:49:20
#
#-------------------------------------------------

QT       += core gui
QT += network

TARGET = FlashDevelopBridge
TEMPLATE = app


SOURCES += main.cpp\
        mainwindow.cpp \
    filesystemwatcherex.cpp \
    bridgeserver.cpp \
    bridgethread.cpp

HEADERS  += mainwindow.h \
    filesystemwatcherex.h \
    bridgeserver.h \
    bridgethread.h \
    simplelog.h

FORMS    += mainwindow.ui

RESOURCES += \
    res.qrc

ICON = images/FDIcon.icns

QMAKE_INFO_PLIST = DMG/app.plist
