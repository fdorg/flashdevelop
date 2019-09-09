#include <QDebug>
#include <QDir>
#include <QSettings>
#include <QMenu>
#include <QSystemTrayIcon>
#include "simplelog.h"
#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent), ui(new Ui::MainWindow)
{
    QString logPath = QDir::homePath() + "/.fdbridge_log";
    logfile.open(logPath.toUtf8().data());
    qInstallMessageHandler(SimpleLogHandler);
    ui->setupUi(this);
    setWindowFlags(Qt::Dialog | Qt::CustomizeWindowHint | Qt::WindowTitleHint | Qt::WindowCloseButtonHint);
    showOnStart = false;
    initTray();
    initStatusBar();
    initMapping();
    initServer();
    if (showOnStart) show();
}

MainWindow::~MainWindow()
{
    delete server;
    delete ui;
}

/* TRAY ICON */

void MainWindow::initTray()
{
    sti = new QSystemTrayIcon(this);
    QIcon img(":/Images/TrayIcon.png");
    sti->setIcon(img);
    sti->setVisible(true);
    QMenu *stiMenu = new QMenu(this);
    sti->setContextMenu(stiMenu);
    QAction *item = new QAction("Configure", this);
    stiMenu->addAction(item);
    connect(item, SIGNAL(triggered()), this, SLOT(showWindow()));
    item = new QAction("Exit", this);
    stiMenu->addAction(item);
    connect(item, SIGNAL(triggered()), this, SLOT(quit()));
}

void MainWindow::quit()
{
    qApp->exit();
}

void MainWindow::showWindow()
{
    show();
    activateWindow();
    raise();
}

void MainWindow::closeEvent(QCloseEvent *event)
{
    hide();
    event->ignore();
}

/* HELPER SERVER */

void MainWindow::initServer()
{
    QSettings settings;
    settings.beginGroup("server");
    if (!settings.contains("host")) settings.setValue("host", "127.0.0.1");
    if (!settings.contains("port")) settings.setValue("port", 8009);
    server = new BridgeServer();
    if (server->isListening())
    {
        connect(server, SIGNAL(bridgeStatus(int,int)), this, SLOT(bridgeStatus(int,int)));
        statusLabel->setText(QString("Listening to port %1").arg(settings.value("port").toString()) );
        QString msg = QString("Is now listening to port %1").arg(settings.value("port").toString());
        sti->showMessage("FlashDevelop Bridge", msg/*, QSystemTrayIcon::NoIcon*/);
    }
    else statusLabel->setText(QString("Failed to listen to port %1").arg(settings.value("port").toInt()) );
}

/* STATUS BAR */

void MainWindow::initStatusBar()
{
    statusLabel = new QLabel();
    statusLabel->setContentsMargins(10, 0,0,0);
    statusBar = new QStatusBar();
    statusBar->addWidget(statusLabel, 1);
    setStatusBar(statusBar);
}

void MainWindow::bridgeStatus(int threads, int watchers)
{
    QString msg = QString("Running: %1 socket(s), %2 watcher(s)").arg(threads).arg(watchers);
    statusLabel->setText(msg);
}

/* LOCAL REMOTE MAPPING EDITION */

void MainWindow::initMapping()
{
    lockSettings = true;
    QSettings settings;
    settings.beginGroup("localRemoteMap");
    QStringList map = settings.allKeys();
    if (map.length() == 0)
    {
        showOnStart = true;
        settings.setValue("Y", QDir::homePath());
        settings.endGroup();
        settings.beginGroup("localRemoteMap");
        map = settings.allKeys();
    }
    ui->listMap->setRowCount(map.length());
    ui->listMap->setColumnWidth(0, 100);
    int line = 0;
    foreach(QString key, map)
    {
        QTableWidgetItem *item = new QTableWidgetItem(key + ":\\");
        ui->listMap->setItem(line, 0, item);
        item = new QTableWidgetItem(settings.value(key).toString());
        ui->listMap->setItem(line, 1, item);
        line++;
    }
    ui->btRemove->setEnabled(false);
    lockSettings = false;
    updateSettings(); // validate entries
}

void MainWindow::on_btAdd_clicked()
{
    QTableWidgetItem *item;
    int line = ui->listMap->rowCount();
    // already one empty line?
    if (line > 0)
    {
        item = ui->listMap->item(line-1, 0);
        if (item->text().isEmpty() && ui->listMap->item(line-1, 1)->text().isEmpty())
        {
            ui->listMap->setCurrentItem(item);
            ui->listMap->editItem(item);
            return;
        }
    }
    // new line
    lockSettings = true;
    ui->listMap->setRowCount(line + 1);
    item = new QTableWidgetItem("");
    ui->listMap->setItem(line, 1, item);
    item = new QTableWidgetItem("");
    ui->listMap->setItem(line, 0, item);
    ui->listMap->setCurrentItem(item);
    ui->listMap->editItem(item);
    lockSettings = false;
}

void MainWindow::on_btRemove_clicked()
{
    int line = ui->listMap->currentRow();
    if (line >= 0)
    {
        ui->listMap->removeRow(line);
        updateSettings();
    }
}

void MainWindow::on_listMap_itemSelectionChanged()
{
    bool active = ui->listMap->currentItem() && ui->listMap->currentItem()->isSelected();
    ui->btRemove->setEnabled(active);
}

void MainWindow::on_listMap_cellChanged(int, int)
{
    updateSettings();
}

void MainWindow::updateSettings()
{
    if (lockSettings) return;
    QSettings settings;
    settings.beginGroup("localRemoteMap");
    // clear
    foreach(QString key, settings.allKeys()) settings.remove(key);
    // populate from valid lines in grid
    QColor error(QColor::fromRgb(0xff, 0xcc, 0x66));
    QColor valid(ui->listMap->palette().base().color());
    int lines = ui->listMap->rowCount();
    QRegExp reRemote("([H-Z]):\\\\");
    for (int i = 0; i < lines; ++i)
    {
        QTableWidgetItem *item = ui->listMap->item(i, 0);
        QString remote(item->text());
        bool remoteValid = false;
        if (!remote.isEmpty())
        {
            if (reRemote.indexIn(remote) < 0)
            {
                // invalide remote path
                item->setBackgroundColor(error);
                item->setToolTip("Path doesn't match '<drive letter>:\\' (drive allowed: H to Z)");
            }
            else
            {
                remote = reRemote.cap(1);
                item->setBackgroundColor(valid);
                item->setToolTip("");
                remoteValid = true;
            }
        }
        item = ui->listMap->item(i, 1);
        QString local(item->text());
        bool localValid = false;
        if (!local.isEmpty())
        {
            if (!QDir(local).exists())
            {
                // invalid local path
                item->setBackgroundColor(error);
                item->setToolTip("Path doesn't seem to exist on this computer");
            }
            else
            {
                item->setBackgroundColor(valid);
                item->setToolTip("");
                localValid = true;
            }
        }
        if (remoteValid && localValid) settings.setValue(remote, local);
    }
}
