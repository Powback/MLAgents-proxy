class 'Agent'

function Agent:__init(p_PlayerName)
	print("Initializing Agent: " .. p_PlayerName)
	self.m_PlayerName = p_PlayerName
	self.m_Player = PlayerManager:GetPlayerByName(p_PlayerName)
	if(self.m_Player == nil) then
		print("Failed to get player")
		return nil
	end
	self:RegisterVars()
	self:RegisterEvents()
end


function Agent:RegisterVars()
	self.alive = false
	local transform = self.m_Player.soldier.transform
	self.rot = MathUtils:GetYPRFromULF(transform.up, transform.left, transform.forward)
end
function Agent:RegisterEvents()
	MLAgents:RegisterAgentCallback('agentReset', self.m_Player.id, self.OnReset)
	MLAgents:RegisterAgentCallback('agentAction', self.m_Player.id, self.OnAction)
	MLAgents:RegisterAgentCallback('agentHeuristic', self.m_Player.id, self.OnHeuristic)
end
function Agent:Update(p_Delta)
	if(self.m_Player == nil) then
		return
	end
	self.alive = (self.m_Player.soldier ~= nil)
	--self.m_Player.input:SetLevel(EntryInputActionEnum.EIAYaw, 0.75)
end

function Agent:OnReset(p_Command)

end

function Agent:OnReset(p_Command)

end

