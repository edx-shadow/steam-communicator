using System;
using System.Timers;
using Steamworks;

namespace SteamCommunicator
{
	// Token: 0x02000002 RID: 2
	internal static class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			Environment.SetEnvironmentVariable("SteamAppId", "212480");
			Callback<LobbyMatchList_t>.Create(new Callback<LobbyMatchList_t>.DispatchDelegate(Program.OnGetLobbiesList));
			Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(Program.OnLobbyEnter));
			if (SteamAPI.Init())
			{
				if (args.Length != 0)
				{
					string a = args[0];
					if (!(a == "lobbylist"))
					{
						if (!(a == "playerlist"))
						{
							if (!(a == "friendlist"))
							{
								if (!(a == "migratehost"))
								{
									if (!(a == "setplayerlimit"))
									{
										Program.finished = true;
									}
									else
									{
										SteamMatchmaking.SetLobbyMemberLimit((CSteamID)Convert.ToUInt64(args[1]), Convert.ToInt32(args[2]));
										Program.finished = true;
									}
								}
								else
								{
									SteamMatchmaking.SetLobbyOwner((CSteamID)Convert.ToUInt64(args[1]), (CSteamID)Convert.ToUInt64(args[2]));
									Program.finished = true;
								}
							}
							else
							{
								Program.GetFriendList();
							}
						}
						else
						{
							SteamMatchmaking.JoinLobby((CSteamID)Convert.ToUInt64(args[1]));
						}
					}
					else
					{
						SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
						SteamMatchmaking.RequestLobbyList();
					}
				}
				else
				{
					Program.finished = true;
				}
				Program._Timer.Elapsed += Program.TimeOut;
				Program._Timer.Start();
				while (!Program.finished && !Program.timedOut)
				{
					SteamAPI.RunCallbacks();
				}
				if (Program.timedOut)
				{
					Console.WriteLine("-2");
				}
				SteamAPI.Shutdown();
				return;
			}
			Console.WriteLine("-1");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000021C4 File Offset: 0x000003C4
		private static void OnGetLobbiesList(LobbyMatchList_t result)
		{
			Console.WriteLine(result.m_nLobbiesMatching);
			int num = 0;
			while ((long)num < (long)((ulong)result.m_nLobbiesMatching))
			{
				CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(num);
				Console.WriteLine(lobbyByIndex);
				string lobbyData = SteamMatchmaking.GetLobbyData(lobbyByIndex, "name");
				try
				{
					Console.WriteLine(lobbyData.Substring(0, lobbyData.Length - 8));
				}
				catch
				{
					Console.WriteLine("Unknown");
				}
				try
				{
					Console.WriteLine((int)(Convert.ToInt16(SteamMatchmaking.GetLobbyData(lobbyByIndex, "type")) - 1549));
				}
				catch
				{
					Console.WriteLine(4);
				}
				Console.WriteLine(SteamMatchmaking.GetNumLobbyMembers(lobbyByIndex));
				Console.WriteLine(SteamMatchmaking.GetLobbyMemberLimit(lobbyByIndex));
				Console.WriteLine(SteamMatchmaking.GetLobbyData(lobbyByIndex, "lobbydata"));
				num++;
			}
			Program.finished = true;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000022A8 File Offset: 0x000004A8
		private static void OnLobbyEnter(LobbyEnter_t result)
		{
			int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)result.m_ulSteamIDLobby);
			CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner((CSteamID)result.m_ulSteamIDLobby);
			Console.WriteLine(Math.Max(0, numLobbyMembers - 1));
			Console.WriteLine(lobbyOwner);
			Console.WriteLine(SteamFriends.GetFriendPersonaName(lobbyOwner));
			CSteamID steamID = SteamUser.GetSteamID();
			for (int i = 0; i < numLobbyMembers; i++)
			{
				CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)result.m_ulSteamIDLobby, i);
				if (lobbyMemberByIndex != steamID)
				{
					Console.WriteLine(lobbyMemberByIndex);
					Console.WriteLine(SteamFriends.GetFriendPersonaName(lobbyMemberByIndex));
				}
			}
			SteamMatchmaking.LeaveLobby((CSteamID)result.m_ulSteamIDLobby);
			Program.finished = true;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002352 File Offset: 0x00000552
		private static void TimeOut(object source, ElapsedEventArgs e)
		{
			Program.timedOut = true;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000235C File Offset: 0x0000055C
		private static void GetFriendList()
		{
			int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
			Console.WriteLine(friendCount);
			for (int i = 0; i < friendCount; i++)
			{
				CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
				Console.WriteLine(friendByIndex);
				Console.WriteLine(SteamFriends.GetFriendPersonaName(friendByIndex));
			}
			Program.finished = true;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000023A4 File Offset: 0x000005A4
		private static bool FriendListContains(CSteamID[] friendList, CSteamID friendToFind)
		{
			for (int i = 0; i < friendList.Length; i++)
			{
				if (friendList[i] == friendToFind)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000001 RID: 1
		private static Timer _Timer = new Timer(5000.0);

		// Token: 0x04000002 RID: 2
		private static bool finished = false;

		// Token: 0x04000003 RID: 3
		private static bool timedOut = false;
	}
}
