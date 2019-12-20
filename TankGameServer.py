import os
import shutil
import socket
import select
import json

class MsgDeal:
	def __init__(self, server):
		self.server = server
	def onDeal(self, data, player):
		if (getattr(self, data["msgType"])):
			getattr(self, data["msgType"])(data["msgVal"], player)
	def CSMsgReady(self, data, player):
		self.server.sendGameMSG(player.getSocket(), "SCMsgReady", json.dumps({"code":1}))
		allPlayerInfo = self.server.getAllPlayerInfo()
		self.server.notifyAll("SCMsgWaitList", json.dumps({"result":allPlayerInfo}))
		

class GamePlayer:
	def __init__(self, client, id, name):
		self.playerId = id
		self.socket = client
		self.name = name
	def getSocket(self):
		return self.socket
	def getPlayerId(self):
		return self.playerId
	def getPlayerInfo(self):
		return {"playerId":self.playerId, "name":self.name}

class TankServer:
	def __init__(self):
		self.count = 10000
		self.players = {}
		self.sockArr = []
		self.msgDeal = MsgDeal(self)
		self.listenSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.listenSocket.bind(("192.168.1.10",19904))
		self.listenSocket.listen(5)
		self.sockArr.append(self.listenSocket)
	def start(self):
		while True:
			r, w, e = select.select(self.sockArr, [], [], 10)
			self.loopAllSocket(r)
	def loopAllSocket(self, reableSockets):
		for sock in reableSockets:
			if (sock == self.listenSocket):
				self.onEvtNewClientEnter(sock)
			else:
				self.onEvtRecvMsg(sock)
		
	def onEvtNewClientEnter(self, sock):
		conn, addr = sock.accept()
		self.sockArr.append(conn)
		self.createPlayer(conn, addr[0])
	def onEvtRecvMsg(self, sock):
		player = self.getPlayer(sock)
		if (not player):
			return
		data = sock.recv(1024)
		if (data == ""):
			self.closeSocket(sock)
			self.removePlayer(player.getPlayerId())
			return
		datas = data.split(";")
		for res in datas:
			if (res == ""):
				continue
			res = json.loads(res.decode("utf8"))
			self.msgDeal.onDeal(res, player)
	def sendGameMSG(self, sock, msgType, msgVal):
		result = {}
		result["msgType"] = msgType
		result["msgVal"] = msgVal
		self.sendMsg(sock, json.dumps(result))
	def sendMsg(self, sock, msg):
		msg = msg+";"
		print("send msg:"+msg)
		sock.sendall(bytes(msg))
	def notify(self, playerId, msg):
		if (self.players[playerId]):
			self.sendMsg(self.players[playerId].getSocket(), msg)
	def notifyAll(self, msgType, msgVal):
		result = {}
		result["msgType"] = msgType
		result["msgVal"] = msgVal
		msg = json.dumps(result)
		print("msg:"+msg)
		for key in self.players:
			self.notify(key, msg)
	def closeSocket(self, sock):
		for i in range(0, len(self.sockArr)):
			if (self.sockArr[i] == sock):
				del self.sockArr[i]
				break
	def getPlayer(self, socket):
		for key in self.players:
			if self.players[key].getSocket() == socket:
				return self.players[key]
	def createPlayer(self, socket, name):
		print("create player id:"+str(self.count))
		playerId = self.count
		self.players[playerId] = GamePlayer(socket, playerId, name)
		self.count = self.count+1;
	def removePlayer(self, playerId):
		print("remove player id:"+str(playerId))
		del self.players[playerId]
	def getAllPlayerInfo(self):
		data = []
		for key in self.players:
			data.append(self.players[key].getPlayerInfo())
		return data

if __name__ == "__main__":
	server = TankServer()
	server.start()