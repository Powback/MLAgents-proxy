class 'Academy'
require('__shared/Agent')

function Academy:__init()
	print("Initializing Academy")
	self:RegisterVars()
	self:RegisterEvents()
end


function Academy:RegisterVars()
	self.m_Agents = {}
end

function Academy:RegisterEvents()
	MLAgents:RegisterCommand('academyReset', self.OnReset)
	Events:Subscribe('Player:Chat', self, self.OnChat)
end

function Academy:OnChat(p_Player)
	self:SpawnAgents()
end

function Academy:OnExtensionLoaded()
end

function Academy:SpawnAgents()
	print("Spawn")
	local team = 1
	local s_Iterator = EntityManager:GetIterator('ServerCharacterSpawnEntity')
	if s_Iterator == nil then
		print('Failed to get camera iterator')
	else
		local s_Entity = s_Iterator:Next()
		while s_Entity ~= nil do
			s_Entity = SpatialEntity(s_Entity)
		    local bot = {
		        name = "Bot" .. #self.m_Agents,
		        teamId = team,
		        squadId = 0,
		    }
		    local rotation = Vec3(MathUtils:GetRandom(-180, 180), MathUtils:GetRandom(-70, 70), 0)
		    Events:Dispatch('Bots:Spawn', bot.name, bot.teamId, bot.squadId, s_Entity.transform.trans, rotation)
			Components:AddComponentToPlayer(bot.name, Agent)
			if(team == 1) then
				team = 2
			else
				team = 1
			end
			local player = PlayerManager:GetPlayerByName(bot.name)
			table.insert(self.m_Agents, player)
			MLAgents:SendCommand('spawnAgent', {
				position = player.soldier.transform.trans,
				rotation = rotation
			})
			s_Entity = s_Iterator:Next()
		end
	end
end
function Academy:OnReset(p_Command)

end

if g_Academy == nil then
	g_Academy = Academy()
end

return g_Academy