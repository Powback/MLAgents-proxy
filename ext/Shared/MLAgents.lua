class 'MLAgents'


function MLAgents:__init()
	print("Initializing MLAgents")
	self:RegisterVars()
	self:RegisterEvents()
end


function MLAgents:RegisterVars()
	self.m_Connected = false
	self.m_CommandCallbacks = {}
	self.m_AgentCallbacks = {}

	self.m_Commands = {}
	self.m_Socket = Net:Socket(NetSocketFamily.INET, NetSocketType.Datagram)
end


function MLAgents:RegisterEvents()
	Events:Subscribe('UpdateManager:Update', self, self.OnUpdatePass)
end

function MLAgents:RegisterAgentCallback(p_Id, p_Type, p_Callback)
	if(self.m_AgentCallbacks[p_Id] == nil) then
		self.m_AgentCallbacks[p_Id] = {}
	end
	self.m_AgentCallbacks[p_Id][p_Type] = p_Callback
end

function MLAgents:RegisterCommand(p_Type, p_Callback)
	self.m_Commands[p_Type] = p_Callback
end

function MLAgents:OnUpdatePass(p_Delta, p_Pass)
	if(connected and p_Pass ~= UpdatePass.UpdatePass_PreSim) then
		return
	end
	local responseLength, result = self.m_Socket:Read(10)
	if(responseLength == ''  or responseLength == 0 or result ~= 0) then
		return false
	else
		print("Received data of length: " .. responseLength)
		if(responseLength == "0") then
			return true
		end
		local responseRaw, result = self.m_Socket:Read(tonumber(responseLength)) -- str(response.length)  
		if(result ~= 0) then
			print("Failed to receive a response: " .. result)
			return false
		end
		if(responseRaw == "ok") then
			return true
		end
		local response = json.decode(responseRaw)
		if(response == nil) then
			print("Failed to decode a response:")
			print(responseRaw)
			return false
		end
		-- Callback by type
		if(response.type ~= nil and self.m_CommandCallbacks[response.type] ~= nil) then
			self.m_Commands[response.type](response)
		end
		-- Callback by Id
		if(response.id ~= nil and self.m_CommandCallbacks[response.id] ~= nil and self.m_CommandCallbacks[response.id] ~= false) then
			self.m_CommandCallbacks[response.id](response)
		end

		-- Callback by AgentId
		if(response.AgentId ~= nil and self.m_AgentCallbacks[response.AgentId] ~= nil) then
			self.m_AgentCallbacks[response.AgentId][response.type](response)
		end

	end
end

function MLAgents:SendCommand(p_Type, p_Args, p_Callback)
	if(p_Args == nil) then
		p_Args = {}
	end
	if(p_Callback == nil) then
		p_Callback = false
	end
	table.insert(self.m_CommandCallbacks, p_Callback)
	print("Sending command of type " .. p_Type)
	local command = {
		responseType = "command", 
		type = p_Type,  
		id = #self.m_CommandCallbacks
	}
	for k,v in pairs(p_Args) do
		command[k] = v
	end
	local length, result = self.m_Socket:Write(json.encode(command))
	if(result ~= 0) then
		print("Failed to send command: " .. result)
		return false
	end
	print("Attempting to get a response")
	local s_Response = self:OnUpdatePass(nil,nil,true)
	print(s_Response)
	print("Sent command")
	return true
end

function MLAgents:Connect()
	print("Connecting")
	local s_ConnectionStatus = self.m_Socket:Connect("127.0.0.1", 1337)
	if(s_ConnectionStatus == 0) then
		print("Connected!")
		self:SendCommand('connect', nil, function(response)
			print(response)
		end)
	end
end

if g_MLAgents == nil then
	g_MLAgents = MLAgents()
end

return g_MLAgents