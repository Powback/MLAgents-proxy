class 'MLAgentsServer'
MLAgents = require('__shared/MLAgents')
Academy = require('__shared/Academy')
Components = require('__shared/Components')

function MLAgentsServer:__init()
	print("Initializing MLAgentsServer")
	self:RegisterVars()
	self:RegisterEvents()
end

function MLAgentsServer:RegisterVars()
	self.connected = false
end

function MLAgentsServer:RegisterEvents()
	Events:Subscribe('UpdateManager:Update', self, self.OnUpdatePass)
	Events:Subscribe('Extension:Loaded', self, self.OnExtensionLoaded)
end

function MLAgentsServer:OnUpdatePass(p_Delta, p_Pass)
	if(self.connected and p_Pass ~= UpdatePass.UpdatePass_PreSim) then
		return
	end
	Components:OnUpdate(p_Delta, p_Pass)
end

function MLAgentsServer:OnExtensionLoaded()
	MLAgents:Connect()
	self.connected = true
	Academy:OnExtensionLoaded()
end

g_MLAgentsServer = MLAgentsServer()

