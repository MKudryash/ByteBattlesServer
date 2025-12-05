# Этап 1: сборка фронтенда
FROM node:18-alpine as builder

WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# Этап 2: развёртывание через Nginx
FROM nginx:alpine

COPY --from=builder /app/dist /usr/share/nginx/html

# Заменяем дефолтный конфиг Nginx
COPY nginx/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

