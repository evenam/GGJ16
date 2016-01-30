package server;

import java.net.*;

public class ServerMain 
{
	ServerSocket server;
	final int port = 8904;
	final int maxConnections = 100; // no-one wants to play this game
	
	private ClientConnection[] connections;
	int numConnections;
	
	public static void main(String[] args)
	{
		new ServerMain();
	}
	
	public ServerMain()
	{
		int i = 0;
		numConnections = 0;
		connections = new ClientConnection[maxConnections];
		
		// create server socket
		try 
		{
			server = new ServerSocket(port);
			System.out.println("Listening on port " + port);
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
		
		// continuously accept connections
		while (true)
		{
			try
			{
				Socket sock = server.accept();
				addConnection(new ClientConnection(i++, sock, this));
			}
			catch (Exception e)
			{
				e.printStackTrace();
			}
		}
		
		//server.close();
	}
	
	synchronized void addConnection(ClientConnection connection)
	{
		int index = numConnections;
		if (index >= maxConnections) return;
		
		connections[index] = connection;
		numConnections++;
	}
	
	synchronized void removeConnection(ClientConnection connection)
	{
		for (int i = 0; i < numConnections; i++)
		{
			if (connection == connections[i])
			{
				for (int j = i; j < numConnections - 1; j++)
				{
					connections[j] = connections[j+1];
				}
				connections[numConnections - 1] = null;
				numConnections--;
			}
		}
	}
	
	synchronized ClientConnection getConnection(String name)
	{
		for (int i = 0; i < numConnections; i++)
		{
			if (connections[i].getName() != null)
			{
				if (connections[i].getName().equals(name))
				{
					return connections[i];
				}
			}
		}
		return null;
	}
}
