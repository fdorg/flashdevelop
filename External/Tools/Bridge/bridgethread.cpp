#include <QtNetwork>
#include <QDebug>
#include <QSettings>
#include "bridgethread.h"
#define EOL "*"

bool BridgeThread::pathNotified = false;

BridgeThread::BridgeThread(int descriptor, QObject *parent) : QObject(parent)
{
    socketDescriptor = descriptor;
    handler = 0;
    client = new QTcpSocket();
    if (!client->setSocketDescriptor(socketDescriptor))
    {
        qDebug() << "Socket init error:" << client->errorString();
        return;
    }
    if (!BridgeThread::pathNotified)
    {
        qDebug() << "Notify bridge path...";
        BridgeThread::pathNotified = true;
        QString path = QCoreApplication::applicationDirPath();
        BridgeThread::sendMessage("BRIDGE:" + path.toUtf8());
    }
    connect(client, SIGNAL(disconnected()), this, SLOT(client_disconnected()));
    connect(client, SIGNAL(readyRead()), this, SLOT(client_readyRead()));
    timer.setSingleShot(true);
    connect(&timer, SIGNAL(timeout()), this, SLOT(timer_elapsed()));
}

BridgeThread::~BridgeThread()
{
    disconnect(&timer, SIGNAL(timeout()), this, SLOT(timer_elapsed()));
    disconnect(client, SIGNAL(disconnected()), this, SLOT(client_disconnected()));
    disconnect(client, SIGNAL(readyRead()), this, SLOT(client_readyRead()));
    delete client;
}

void BridgeThread::timer_elapsed()
{
    QMutexLocker locker(&mutex);
    if (queue.length() > 0)
    {
        foreach(QString message, queue)
        {
            qDebug() << message;
            client->write(message.toUtf8());
            client->write(EOL);
        }
        client->flush();
        queue.clear();
    }
}

void BridgeThread::client_readyRead()
{
    QString msg(client->readLine().trimmed());
    int colon = msg.indexOf(':');
    if (colon > 0)
    {
        qDebug() << "Received:" << msg;
        QString cmd = msg.mid(0, colon);
        emit command(cmd, msg.mid(colon + 1));
        if (cmd == "close" || cmd == "unwatch") client_disconnected();
    }
}

void BridgeThread::client_disconnected()
{
    qDebug() << "Socket disconnected";
    emit disconnected();
}

void BridgeThread::sendMessage(QString message)
{
    QMutexLocker locker(&mutex);
    if (!queue.contains(message)) queue << message;
    timer.start(100);
}

/* COMMANDS HANDLING */

BridgeHandler::BridgeHandler(QObject *parent) : QObject(parent)
{
    fsw = 0;
    useCount = 0;
    isSpecial = false;
}

BridgeHandler::~BridgeHandler()
{
    if (fsw != 0)
    {
        qDebug() << "disposed watcher " << fsw->path();
        delete fsw;
    }
}

void BridgeHandler::command(QString name, QString param)
{
    if (name == "watch") watchPath(param);
}

void BridgeHandler::watchPath(QString param)
{
    watched = param;
    QStringList hasFilter(param.split('*'));
    if (hasFilter.length() > 1)
    {
        filter << "*" + hasFilter[1];
        remotePath = hasFilter[0].replace('\\', '/');
    }
    else remotePath = param.replace('\\', '/');
    localPath = getLocalPath(remotePath);
    bool isDir = localPath.endsWith('/');
    if (isDir)
    {
        remotePath.chop(1);
        localPath.chop(1);
    }
    watchedPath = localPath;
    qDebug() << "watch" << watchedPath << filter;
    fsw = new FileSystemWatcherEx(this);
    connect(fsw, SIGNAL(fileSystemChanged(QString)), this, SLOT(localChanged(QString)));
    if (isSpecial)
    {
        watchedPath = getSpecialPath();
        fsw->setPath(watchedPath, false);
    }
    else if (isDir) fsw->setPath(watchedPath, true);
    else fsw->setFile(watchedPath);
}

QString BridgeHandler::getSpecialPath()
{
    QString path("");
    if (special == "flashide")
    {
        path = QDir::home().absolutePath() + "/Library/Application Support/Adobe/FlashDevelop/";
        QDir().mkpath(path);
    }
    return path;
}

QString BridgeHandler::getLocalPath(QString path)
{
    QString local = "";
    QSettings settings;
    settings.beginGroup("localRemoteMap");
    foreach(QString key, settings.allKeys())
    {
        QString remoteRoot = QString(key + ":/");
        if (path.startsWith(remoteRoot))
        {
            local = settings.value(key).toString() + "/" + path.mid(remoteRoot.length());
            break;
        }
    }
    if (local.isEmpty()) local = path;
    if (local.contains(".FlashDevelop"))
    {
        special = QDir(local).dirName();
        isSpecial = true;
        qDebug() << "Special:" << special;
    }
    return local;
}

QString BridgeHandler::getRemotePath(QString path)
{
    if (isSpecial) return remotePath;
    else
    {
        int len = localPath.length();
        if (path.length() < len) return "";
        QString sub = path.mid(len);
        if (sub == "/obj") return "";
        return QString(remotePath + sub).replace('/', '\\');
    }
}

void BridgeHandler::localChanged(QString path)
{
    if (isSpecial) // copy in shared space
    {
        qDebug() << "Copy shared:" << path;
        QDir dir(path);
        dir.setNameFilters(filter);
        QStringList files(dir.entryList());
        if (files.length() == 0) return; // no file matches filter
        foreach(QString name, files)
        {
            copyPatched(path + name, localPath + "/" + name);
        }
        path = localPath;
    }
    path = getRemotePath(path);
    if (path.length() > 0)
    {
        qDebug() << "Local changed:" << path;
        emit notifyChanged(path);
    }
}

void BridgeHandler::copyPatched(QString filePath, QString destPath)
{
    QFile file(filePath);
    if (!file.open(QIODevice::ReadOnly)) return;
    QString src = QString::fromUtf8(file.readAll().data());
    file.close();
    // replace local paths by remote paths
    QSettings settings;
    settings.beginGroup("localRemoteMap");
    foreach(QString key, settings.allKeys())
    {
        QString remote(key + ":\\");
        QString local(settings.value(key).toString());
        src = src.replace(local, remote);
    }
    QFile dest(destPath);
    if (!dest.open(QIODevice::WriteOnly)) return;
    dest.write(src.toUtf8());
    dest.close();
}
