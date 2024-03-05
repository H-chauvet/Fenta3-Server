﻿using shared;

namespace server
{
	/**
	 * The LoginRoom is the first room clients 'enter' until the client identifies himself with a PlayerJoinRequest. 
	 * If the client sends the wrong type of request, it will be kicked.
	 *
	 * A connected client that never sends anything will be stuck in here for life,
	 * unless the client disconnects (that will be detected in due time).
	 */ 
	class LoginRoom : SimpleRoom
	{
		//arbitrary max amount just to demo the concept
		private const int MAX_MEMBERS = 50;

		public LoginRoom(TCPGameServer pOwner) : base(pOwner)
		{
		}

		protected override void addMember(TcpMessageChannel pMember)
		{
			base.addMember(pMember);

		}

		protected override void handleNetworkMessage(ASerializable pMessage, TcpMessageChannel pSender)
		{
			if (pMessage is PlayerJoinRequest)
			{
				handlePlayerJoinRequest(pMessage as PlayerJoinRequest, pSender);
			}
			else //if member sends something else than a PlayerJoinRequest
			{
				Log.LogInfo("Declining client, auth request not understood", this);

				//don't provide info back to the member on what it is we expect, just close and remove
				removeAndCloseMember(pSender);
			}
		}

		/**
		 * Tell the client he is accepted and move the client to the lobby room.
		 */
		private void handlePlayerJoinRequest (PlayerJoinRequest pMessage, TcpMessageChannel pSender)
		{
			if(!_server.IsPlayerInServer(pSender))
			{
				if(_server.GetPlayerInfo(pInfo => pInfo.playerName == pMessage.name).Count > 0)
				{
                    PlayerJoinResponse playerDeniedResponse = new PlayerJoinResponse();
					playerDeniedResponse.result = PlayerJoinResponse.RequestResult.NOTUNIQUE;
                    pSender.SendMessage(playerDeniedResponse);
					return;
                }
                Log.LogInfo("Moving new client to accepted...", this);
				PlayerInfo playerInfo = new PlayerInfo();
				playerInfo.playerName = pMessage.name;

				PlayerJoinResponse playerJoinResponse = new PlayerJoinResponse();
                playerJoinResponse.result = _server.AddPlayerToServer(pSender, playerInfo);
                pSender.SendMessage(playerJoinResponse);

                removeMember(pSender);
                _server.GetDataHandler().AddMember(pSender);
            }
		}

	}
}
