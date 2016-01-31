package server;

import java.net.*;
import java.io.*;

public class ClientConnection implements Runnable 
{
	private Socket socket;
	private String name;
	private boolean running;
	private PrintStream out;
	private BufferedReader in;
	private int threadNumber;
	private ServerMain app;
	private ClientConnection opponent;
	private String opponentName;
	Stage stage; 
	
	private enum Stage
	{ 
		GETTING_USERNAME, 
		GETTING_OPPONENTNAME, 
		WAITING_CLIENT, 
		WAITING_OPPONENT 
	}
	
	public ClientConnection(int num, Socket sock, ServerMain application)
	{
		threadNumber = num;
		socket = sock;
		app = application;
		System.out.println("Accepted user on thread " + num);
		opponentName = "";
		
		try
		{
			in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			out = new PrintStream(socket.getOutputStream());
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
		
		Thread t = new Thread(this);
		t.start();
	}
	
	
	public void run() 
	{
		running = true;
		stage = Stage.GETTING_USERNAME;
		while (running)
		{
			if (socket.isConnected())
			{
				String input = "";
				try 
				{
					input = in.readLine();
				}
				catch (Exception e)
				{
					e.printStackTrace();
					running = false;
				}
				
				if (!input.equals("DISCONNECT"))
				{
					if (input.equals("GAMEOVER"))
					{
						stage = Stage.GETTING_USERNAME;
						try
						{
							out.println("GAMEOVER");
							stage = Stage.GETTING_USERNAME;
						}
						catch (Exception e)
						{
							e.printStackTrace();
						}
					}
					else if (stage == Stage.GETTING_USERNAME) 
					{
						attemptGetUsername(input);
						continue;
					}
					else if (stage == Stage.GETTING_OPPONENTNAME) 
					{
						attemptGetOpponentname(input);
						continue;
					}
					else if (stage == Stage.WAITING_CLIENT) 
					{
						attemptClientWait(input);
						continue;
					}
				}
				else
				{
					try
					{
						out.println("XDISCONNECT");
					}
					catch (Exception e)
					{
						e.printStackTrace();
					}
					running = false;
				}
			}
			if (!socket.isConnected()) 
				running = false;
		}
		clean();
	}
	
	public void clean()
	{
		System.out.println("Removing user " + getName() + " on thread " + threadNumber);
		app.removeConnection(this);
		try
		{
			in.close();
			out.close();
			socket.close();
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
	
	// grunt workers
	private void attemptGetUsername(String input)
	{
		String username = input;
		ClientConnection test = app.getConnection(username);
		if (test != null)
		{
			out.println("USERNAME_REJECTED");
			out.flush();
			System.out.println("Username of thread " + threadNumber + " (" + username + ") rejected. ");
		}
		else
		{
			System.out.println("Username of thread " + threadNumber + " (" + username + ") accepted. ");
			out.println("USERNAME_ACCPETED");
			out.flush();
			setName(username);
			stage = Stage.GETTING_OPPONENTNAME;
		}
	}
	
	private void attemptGetOpponentname(String input)
	{
		if (opponentName.equals(""))
		{
			opponentName = input;
			out.println("OPPONENT_ACCEPTED");
			out.flush();
			opponent = app.getConnection(opponentName);
			if (opponent != null)
			{
				System.out.println("Opponent of thread " + threadNumber + " (" + opponentName + ") accepted. ");
				opponent.setOpponent(this);
				opponent.goFirst();
				goSecond();
			}
			else
			{
				System.out.println("Waiting on opponent in thread " + threadNumber + " (" + opponentName + "). ");
				stage = Stage.GETTING_OPPONENTNAME;
			}
		}
	}
	
	synchronized private void attemptClientWait(String input)
	{
		System.out.println("Sending opponent a game move: " + input);
		stage = Stage.WAITING_OPPONENT;
		if (opponent == null)
			opponent = app.getConnection(opponentName);
		opponent.opponentResponse(input);
	}
	
	synchronized void opponentResponse(String resp)
	{
		try
		{
			System.out.println("Received a game move: " + resp);
			out.println(resp);
			out.flush();
			stage = Stage.WAITING_CLIENT;
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
	
	synchronized public String getName()
	{
		return name;
	}
	
	synchronized public void setName(String newName)
	{
		name = newName;
	}
	
	synchronized public void goFirst()
	{
		stage = Stage.WAITING_CLIENT;
		System.out.println(name + "(" + threadNumber + ") going first. ");
		out.println("FIRST");
		out.flush();
	}
	
	synchronized public void goSecond()
	{
		stage = Stage.WAITING_OPPONENT;
		System.out.println(name + "(" + threadNumber + ") going second. ");
		out.println("SECOND");
		out.flush();
	}
	
	synchronized public void setOpponent(ClientConnection c)
	{
		opponent = c;
	}
}
