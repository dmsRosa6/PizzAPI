#!lua name=orderlib

-- STATUS CONSTANTS
local STATUS_STARTED = "Started"
local STATUS_COOKING = "Cooking"
local STATUS_DELIVERING = "Delivering"
local STATUS_DELIVERED = "Delivered"

local allowed = {
    [STATUS_STARTED] = true,
    [STATUS_COOKING] = true,
    [STATUS_DELIVERING] = true,
    [STATUS_DELIVERED] = true
}

-- DELIMITERS
local ORDER_STATUS_KEYWORD = ".status"
local ORDER_HISTORY_KEYWORD = ".history"
local ORDER_LOCATION_KEYWORD = ".location"
local UPDATE_CHANNEL_KEYWORD = ".chan"
local DELIMITER = ":"

-- UPDATE CHANNEL MESSAGES
local CREATED_MESSAGE = "Created"
local UPDATED_MESSAGE = "Updated"
local DELETED_MESSAGE = "Deleted"

-- HELPER FUNCTIONS

local function get_keys(order_id)
    return {
        status = order_id .. ORDER_STATUS_KEYWORD,
        history = order_id .. ORDER_HISTORY_KEYWORD,
        location = order_id .. ORDER_LOCATION_KEYWORD,
        chan = order_id .. UPDATE_CHANNEL_KEYWORD
    }
end

-- EXPOSED FUNCTIONS

-- Updates a order in cache
-- keys: {order_id}
-- args: {latitude, longitude}
local function create_order(keys, args)
    local order_id = keys[1]
    local latitude = args[1]
    local longitude = args[2]

    if not order_id then
        error("Order ID is required.")
    end

    if #args ~= 2 then
        error("Invalid number of args. Expected 2, got: " .. #args)
    end

    local order_structs = get_keys(order_id)

    local res = redis.call('SET', order_structs.status, STATUS_STARTED, "NX")

    if not res then
        error("Order already exists: OrderId: " .. order_id)
    end

    redis.call('RPUSH', order_structs.history, STATUS_STARTED)   
    redis.call('SET', order_structs.location, latitude .. DELIMITER .. longitude)

    local update_payload = cjson.encode({
        message = CREATED_MESSAGE,
        orderId = order_id,
        orderStatus = STATUS_STARTED,
        coordinates = {
            latitude = tonumber(latitude),
            longitude = tonumber(longitude)
        }
    })

    redis.call('PUBLISH', order_structs.chan, update_payload)

    return "OK"
end

-- Updates a order in cache
-- keys: {order_id}
-- args: {new_status, latitude, longitude}
local function update_order(keys, args)
    local order_id = keys[1]
    local new_status = args[1]
    local latitude = args[2]
    local longitude = args[3]

    if not order_id then
        error("Order ID is required.")
    end

    if #args ~= 3 then
        error("Invalid number of args. Expected 3, got: " .. #args)
    end

    if not allowed[new_status] then
        error("Invalid status: " .. new_status)
    end

    local order_structs = get_keys(order_id)

    local res = redis.call('SET', order_structs.status, new_status, "XX")

    if not res then
        error("The Order does not exist. OrderId: " .. order_id)
    end

    redis.call('RPUSH', order_structs.history, new_status)
    redis.call('SET', order_structs.location, latitude .. DELIMITER .. longitude)

    local update_payload = cjson.encode({
        message = UPDATED_MESSAGE,
        orderId = order_id,
        orderStatus = new_status,
        coordinates = {
            latitude = tonumber(latitude),
            longitude = tonumber(longitude)
        }
    })

    redis.call('PUBLISH', order_structs.chan, update_payload)

    return "OK"
end


-- Terminates an order by deleting all the data structs that define it (in cache)
-- keys: {order_id}
-- args: {}
local function end_order(keys, args)
    local order_id = keys[1]

    if not order_id then
        error("Order ID is required.")
    end

    local order_structs = get_keys(order_id)

    redis.call('DEL', order_structs.status)
    redis.call('DEL', order_structs.history)
    redis.call('DEL', order_structs.location)

    local update_payload = cjson.encode({
        message = UPDATED_MESSAGE,
        orderId = order_id
    })

    redis.call('PUBLISH', order_structs.chan, update_payload)

    return "OK"
end

-- Gets an order >:)
-- keys: {order_id}
-- args: {}
local function get_order(keys,args)
    local order_id = keys[1]

    if not order_id then
        error("Order ID is required.")
    end

    local order_structs = get_keys(order_id)

    local status = redis.call('GET', order_structs.status)

    if not status then
        error("Order does not exists: OrderId: " .. order_id)
    end

    local location = redis.call('GET', order_structs.location)
    local tokens = string.gmatch(location, "[^%s" .. DELIMITER.."]+")
    local latitude = tokens[1]
    local longitude = tokens[2]

    if not (latitude and longitude) then
        error("Latitude/Longitude aren’t valid numbers: " .. latitude .. ", " .. longitude)
    end

    local update_payload = cjson.encode({
        message = UPDATED_MESSAGE,
        orderId = order_id,
        orderStatus = status,
        coordinates = {
            latitude = tonumber(latitude),
            longitude = tonumber(longitude)
        }
    })

    return update_payload
end

-- Register Functions
redis.register_function('create_order', create_order)
redis.register_function('update_order', update_order)
redis.register_function('end_order', end_order)
redis.register_function('get_order', get_order)