# 暗夜软件自动更新

#### 介绍

这是一款 dotnet core 基于C#语言开发,开源跨平台通用自动更新程序

#### 特点


1. 跨平台,轻量级,通用
2. 差异更新,更新速度快
3. 界面管理更新数据,操作简单
4. 运行方式支持:普通运行和服务运行
5. 自动更新支持:可执行程序和服务程序


#### 使用教程

1. 运行服务端:
        a. windows:双击运行[Anye.Soft.AutoUpdate.Server.exe]
        b. linux:先进入[程序目录],之后执行:`bash linux_run.sh`

2. 使用管理端程序:
        a. 运行[Anye.Soft.AutoUpdate.Manage.exe]
        b. 创建更新服务: [新增服务] => 在列表里双击<刚刚新增的服务>
        c. 创建更新库:   => [创建更新库]
        d. 创建程序版本: => [新增版本] => [导入目录] => [发布]

3. 使用命令端程序(先进入程序目录):
	a. windows
		a-1.添加更新库配置:`Anye.Soft.AutoUpdate.Exec.exe add -n app1 -t 127.0.0.1:9999 -u updater -k 123456 -l app1`
		a-2.执行更新:`Anye.Soft.AutoUpdate.Exec.exe update -n app1`
		或.直接执行:`Anye.Soft.AutoUpdate.Exec.exe run -t 127.0.0.1:9999 -u updater -k 123456 -l app1`
	b. linux
		b-1.添加更新库配置:`dotnet Anye.Soft.AutoUpdate.Exec.dll add -n app1 -t 127.0.0.1:9999 -u updater -k 123456 -l app1`
		b-2.执行更新:`dotnet Anye.Soft.AutoUpdate.Exec.dll update -n app1`
		或.直接执行:`dotnet Anye.Soft.AutoUpdate.Exec.dll run -t 127.0.0.1:9999 -u updater -k 123456 -l app1`

(详细请看项目下的[使用说明.txt])

#### 项目说明

1. 服务端:服务提供接口
	使用说明.txt
	Anye.Soft.AutoUpdate.Server.exe #windows程序
	win_install.bat #安装成windows服务
	win_start_service.bat #启动windows服务
	win_stop_service.bat #停止windows服务
	win_uninstall.bat #卸载windows服务
	Anye.Soft.AutoUpdate.Server.dll #linux程序
	anyesoft.autoupdate.service #linux服务配置
	linux_disable_service.sh #linux服务取消开机启动
	linux_enable_service.sh #linux服务开机启动
	linux_restart_service.sh #linux服务重启
	linux_start_service.sh #linux服务启动
	linux_stop_service.sh #linux服务停止
	linux_run.sh #linux程序后台运行
	log4net.config #日志配置
	server.json #配置文件
	
2. 管理端:连接服务端,管理更新库信息
	使用说明.txt
	Anye.Soft.AutoUpdate.Manage.exe #windows程序
	log4net.config #日志配置
	
3. 命令端:连接服务端,获取更新库信息,实现自动更新程序
	使用说明.txt
	Anye.Soft.AutoUpdate.Exec.exe #windows程序
	Anye.Soft.AutoUpdate.Exec.dll #linux程序
	log4net.config #日志配置
	
	
	
	

