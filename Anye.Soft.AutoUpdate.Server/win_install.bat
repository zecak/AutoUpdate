@echo off
sc.exe create AnyeSoftAutoUpdateServerService displayname=��ҹ����Զ����·��� start=auto binPath=%~dp0Anye.Soft.AutoUpdate.Server.exe
sc.exe description AnyeSoftAutoUpdateServerService "�ṩ�Զ����·���"
sc.exe start AnyeSoftAutoUpdateServerService
@pause