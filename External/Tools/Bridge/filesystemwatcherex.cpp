#include <QDebug>
#include <QDir>
#include "filesystemwatcherex.h"

FileSystemWatcherEx::FileSystemWatcherEx(QObject *parent) : QObject(parent)
{
    basePath = "";
    watchedFile = 0;
}

FileSystemWatcherEx::~FileSystemWatcherEx()
{
    if (basePath != "")
    {
        QStringList dirs = fsw->directories();
        if (dirs.length() > 0) fsw->removePaths(dirs);
        QStringList files = fsw->files();
        if (files.length() > 0) fsw->removePaths(fsw->files());
        delete fsw;
    }
}

void FileSystemWatcherEx::setPath(QString path, bool includeSubdirectories)
{
    if (path == "") return;
    QDir dir(path);
    if (dir.exists())
    {
        qDebug() << "Watching:" << path;
        basePath = path;
        includeSubs = includeSubdirectories;
        fsw = new QFileSystemWatcher(this);
        if (includeSubs) fsw->addPaths(subFoldersList(basePath));
        else fsw->addPath(basePath);
        connect(fsw, SIGNAL(directoryChanged(QString)), this, SLOT(directoryChanged(QString)));
    }
    else qDebug() << "Path not found:" << path;
}

void FileSystemWatcherEx::setFile(QString path)
{
    if (path == "") return;
    QFileInfo file(path);
    if (file.exists())
    {
        qDebug() << "Watching:" << path;
        basePath = file.dir().absolutePath();
        watchedFile = new WatchedFile(file);
        fsw = new QFileSystemWatcher(this);
        fsw->addPath(basePath);
        connect(fsw, SIGNAL(directoryChanged(QString)), this, SLOT(fileChanged(QString)));
    }
    else qDebug() << "Path not found:" << path;
}

// Explore folders in depth and return concaneted list of folder paths

QStringList FileSystemWatcherEx::subFoldersList(QString path)
{
    QDir dir(path);
    dir.setFilter(QDir::Dirs);
    QStringList dirList;
    dirList << dir.absolutePath();
    QFileInfoList list = dir.entryInfoList();
    for (int i = 0; i < list.size(); ++i)
    {
        QFileInfo fileInfo = list.at(i);
        QString name = fileInfo.fileName();
        if (name[0] == '.' || name[0] == '_') continue;
        dirList << subFoldersList(fileInfo.filePath());
    }
    return dirList;
}

void FileSystemWatcherEx::fileChanged(QString path)
{
    if (!watchedFile->file.startsWith(path)) return;
    qDebug() << "Changed:" << path;
    QFileInfo info(watchedFile->file);
    if (info.exists())
    {
        if (!watchedFile->exists)
        {
            watchedFile->exists = true;
            watchedFile->lastModified = info.lastModified();
        }
        else if (watchedFile->lastModified == info.lastModified()) return;
    }
    else
    {
        if (!watchedFile->exists) return;
        watchedFile->exists = false;
    }
    emit fileSystemChanged(watchedFile->file);
    return;
}

void FileSystemWatcherEx::directoryChanged(QString path)
{
    qDebug() << "Changed:" << path;
    QDir dir(path);
    if (dir.exists()) // content changed
    {
        if (includeSubs)
        {
            QStringList dirs = fsw->directories();
            foreach(QString sub, subFoldersList(path))
            {
                if (!dirs.contains(sub)) fsw->addPath(sub);
            }
        }
        emit fileSystemChanged(path);
    }
    else // dir removed/renames -> update parent
    {
        dir.cdUp();
        emit fileSystemChanged(dir.absolutePath());
    }
}

QString FileSystemWatcherEx::path()
{
    return basePath;
}
