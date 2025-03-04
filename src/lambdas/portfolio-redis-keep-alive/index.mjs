import { createClient } from 'redis';

export const handler = async (event) => {

  const username = process.env.REDIS_USERNAME;
  const password = process.env.REDIS_PASSWORD;
  const host = process.env.REDIS_HOST;
  const port = process.env.REDIS_PORT;

  const client = createClient({
    username: username,
    password: password,
    socket: {
        host: host,
        port: port
    }
  });

  client.on('error', err => console.log('Redis Client Error', err));

  await client.connect();

  const now = new Date();
  const formattedDate = now.getFullYear().toString().padStart(4, '0') + 
                        (now.getMonth() + 1).toString().padStart(2, '0') + 
                        now.getDate().toString().padStart(2, '0') + 
                        now.getHours().toString().padStart(2, '0') + 
                        now.getMinutes().toString().padStart(2, '0') + 
                        now.getSeconds().toString().padStart(2, '0');

  const keyName = 'currentDateTime';
                
  try {
    await client.setEx(keyName, 3600, formattedDate);
    const result = await client.get(keyName);
    console.log(result)

    const response = {
      statusCode: 200,
      body: JSON.stringify({
        status: 'ok',
        data: {
          redisResult: result,
          eventData: event
        }
      }),
    };

    return response;
  } catch (error) {
    console.log(error.message);
    return {
      statusCode: 500,
      body: JSON.stringify({
        status: 'error',
        error: error.message,
      }),
    };
  }
};
