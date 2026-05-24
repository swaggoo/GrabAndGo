import random
import uuid
from locust import HttpUser, task, between, events

class GrabAndGoUser(HttpUser):
    wait_time = between(1, 3)
    token = None
    user_id = "6611c298-f744-421e-88b5-99369ce67e52" # customer@example.com

    # Seeded product IDs that exist in the database
    # SHOco. (Business: 602d2149-e773-f2a3-990b-47b000000000)
    product_ids = [
        "602d2149-e773-f2a3-990b-47e000000000",
        "602d2149-e773-f2a3-990b-47e010000000",
        "602d2149-e773-f2a3-990b-47e020000000"
    ]
    
    business_id = "602d2149-e773-f2a3-990b-47b000000000"

    def on_start(self):
        self.login()

    def login(self):
        # Use an existing user from the database seed
        login_payload = {
            "email": "customer@example.com",
            "password": "P@ssw0rd123"
        }
        with self.client.post("/api/auth/login", json=login_payload, catch_response=True) as response:
            if response.status_code == 200:
                data = response.json()
                self.token = data["data"]["token"]
                response.success()
            else:
                response.failure(f"Login failed: {response.status_code} - {response.text}")

    @task
    def place_order(self):
        if not self.token:
            return

        product_id = random.choice(self.product_ids)
        
        payload = {
            "userId": self.user_id,
            "businessId": self.business_id,
            "productId": product_id,
            "totalAmount": 15.50
        }

        headers = {"Authorization": f"Bearer {self.token}"}

        # Targeting the API Gateway
        with self.client.post("/api/orders", json=payload, headers=headers, catch_response=True) as response:
            if response.status_code == 200 or response.status_code == 201:
                response.success()
            elif response.status_code == 401:
                response.failure("Unauthorized - Token might be invalid")
                self.login() # Try to re-login
            else:
                response.failure(f"Order failed with status code: {response.status_code}")

    @task(3)
    def view_catalog(self):
        self.client.get("/api/products")
