@echo off
sc.exe create AnyeSoftAutoUpdateServerService displayname=暗夜软件自动更新服务 start=auto binPath=%~dp0Anye.Soft.AutoUpdate.Server.exe
sc.exe description AnyeSoftAutoUpdateServerService "提供自动更新服务"
sc.exe start AnyeSoftAutoUpdateServerService
@pause