class 'Components'

function Components:__init()
	print("Initializing Components")
	self:RegisterVars()
	self:RegisterEvents()
end

function Components:RegisterVars()
	self.m_Players = {}
	self.m_Entities = {}
end

function Components:RegisterEvents()
end

function Components:AddComponentToPlayer(p_PlayerName, p_Component)
	local player = PlayerManager:GetPlayerByName(p_PlayerName)
	if(player == nil) then
		print("Player does not exist:" .. p_PlayerName)
		return
	end
	if(self.m_Players[p_PlayerName] == nil) then
		self.m_Players[p_PlayerName] = {}
	end
	local s_Component = p_Component(p_PlayerName)
	table.insert(self.m_Players[p_PlayerName], s_Component)
end

function Components:AddComponentToEntity(p_Entity, p_Component)
	if(self.m_Entities[p_Entity.instanceId] == nil) then
		self.m_Entities[p_Entity.instanceId] = {}
	end
	local s_Component = p_Component(p_Entity)
	table.insert(self.m_Players[p_Entity.instanceId], s_Component)
end

function Components:OnUpdate(p_Delta)
	for _,player in pairs(self.m_Players) do
		for _,component in pairs(player) do
			component:Update(p_Delta)
		end
	end
	for _,entity in pairs(self.m_Entities) do
		for _,component in pairs(entity) do
			component:Update(p_Delta)
		end
	end
end

if g_Components == nil then
	g_Components = Components()
end

return g_Components