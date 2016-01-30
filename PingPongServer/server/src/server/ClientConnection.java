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
			if (stage == Stage.GETTING_USERNAME) attemptGetUsername();
			if (stage == Stage.GETTING_OPPONENTNAME) attemptGetOpponentname();
			if (stage == Stage.WAITING_CLIENT) attemptClientWait();
			if (!socket.isConnected()) running = false;
		}
		clean();
	}
	
	public void clean()
	{
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
	private void attemptGetUsername()
	{
		String username;
		try
		{
			username = in.readLine();
			ClientConnection test = app.getConnection(username);
			if (test != null)
			{
				out.println("USERNAME_REJECTED");
				System.out.println("Username of thread " + threadNumber + " (" + username + ") rejected. ");
			}
			else
			{
				System.out.println("Username of thread " + threadNumber + " (" + username + ") accepted. ");
				out.println("USERNAME_ACCPETED");
				setName(username);
				stage = Stage.GETTING_OPPONENTNAME;
			}
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
	
	private void attemptGetOpponentname()
	{
		String opponentName;
		try
		{
			opponentName = in.readLine();
			opponent = app.getConnection(opponentName);
			if (opponent == null)
			{
				out.println("OPPONENT_REJECTED");
				System.out.println("Opponent of thread " + threadNumber + " (" + opponentName + ") rejected. ");
			}
			else
			{
				System.out.println("Opponent of thread " + threadNumber + " (" + opponentName + ") accepted. ");
				out.println("OPPONENT_ACCPETED");
				stage = Stage.WAITING_CLIENT;
			}
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
	
	private void attemptClientWait()
	{
		String input;
		try
		{
			input = in.readLine();
			opponent.opponentResponse(input);
			System.out.println("Sending opponent a game move: " + input);
			stage = Stage.WAITING_OPPONENT;
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
	
	synchronized void opponentResponse(String resp)
	{
		try
		{
			out.println(resp);
			System.out.println("Received a game move: " + resp);
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
}
