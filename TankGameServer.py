import os
import shutil
import socket
import select

class MsgDeal:
	def __init__(self):
		print("")
	def onDeal(self, data, player):
		print("onDeal")

class GamePlayer:
	def __init__(self, client, id):
		self.socket = client
	def getSocket(self):
		return self.socket
		

class TankServer:
	def __init__(self):
		self.count = 10000
		self.players = {}
		self.sockArr = []
		self.msgDeal = MsgDeal()
		self.listenSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.listenSocket.bind(("192.168.1.10",19904))
		self.listenSocket.listen(5)
		self.sockArr.append(self.listenSocket)
	def start(self):
		while True:
			r, w, e = select.select(self.sockArr, [], [], 5)
			print("loop")
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
		self.createPlayer(conn)
	def onEvtRecvMsg(self, sock):
		print("recv msg")
		player = self.getPlayer(sock)
		if (not player):
			return
		data = sock.recv(1024)
		result = self.msgDeal.onDeal(data, player)
		self.sendMsg(sock, result)
		#sock.sendall(bytes("asdawdwd"))
		#reply msg
		#obj.sendall(bytes("asdawdwd"))
	def sendMsg(self, sock, msg):
		sock.sendall(bytes(msg))
	def getPlayer(self, socket):
		for key in self.players:
			if self.players[key].getSocket() == socket:
				return self.players[key]
	def createPlayer(self, socket):
		print("create player id:"+str(self.count))
		playerId = self.count
		self.players[playerId] = GamePlayer(socket, playerId)
		self.count = self.count+1;

if __name__ == "__main__":
	server = TankServer()
	server.start()