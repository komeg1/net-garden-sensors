# Stage 1: Build the Angular application
FROM node:18 AS build

WORKDIR /app

# Copy package.json and package-lock.json files
COPY package*.json ./

# Install dependencies
RUN npm install --legacy-peer-deps

# Copy all Angular application files
COPY . .

# Build the Angular project for production
RUN npm run build --prod

# Stage 2: Serve the Angular application with Nginx
FROM nginx:alpine

# Copy the Nginx configuration template
COPY default.conf.template /etc/nginx/templates/default.conf.template

# Copy the Angular build output to the Nginx web root
COPY --from=build /app/dist/user-app /usr/share/nginx/html

# Generate the final Nginx configuration with envsubst
RUN envsubst < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
