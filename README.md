# LEACH Protocol Simulator (WSN / RCSF)

A visual, real-time simulator for the **LEACH** (*Low-Energy Adaptive Clustering Hierarchy*) protocol in Wireless Sensor Networks (WSN / Réseaux de Capteurs Sans Fil). Built with **C#** and **Windows Forms** (.NET).

---

## 📌 Features

- **Interactive 2D Visualization**: Real-time GDI+ rendering with double buffering.
  - **Base Station (Sink)**: Centralized sink node (red square).
  - **Cluster Heads (CH)**: Dynamically elected cluster heads (green circles with identifier badges).
  - **Member Nodes**: Regular sensing nodes (blue circles) with dashed communication range circles.
  - **Dead Nodes**: Out-of-battery sensors marked in gray.
  - **Battery Life Indicators**: Real-time colored health bars above every node (smooth transition from green to red).
  - **Visual Transmission Links**: Dotted and colored vectors representing member-to-CH, CH-to-Sink, and isolated node transmissions.
- **Three Cluster Head Election Algorithms**:
  1. **Standard LEACH**: Classical probabilistic threshold $T(n)$.
  2. **Energy-Aware LEACH**: Multiplied by the residual energy ratio $\frac{E_{current}}{E_{max}}$.
  3. **Advanced Energy + Starvation Mitigation**: Accounts for rounds without election ($r_s$) to avoid node starvation.
- **Realistic First-Order Radio Energy Model**:
  - Distance threshold ($d_0$) discrimination between Free Space ($\propto d^2$) and Multi-path fading ($\propto d^4$).
  - Packet transmission, reception, and data aggregation energy dissipation.
- **Control & Analytics**:
  - Configurable parameters: Node count, initial battery energy (J), transmission range, and target CH probability ($P$).
  - Step-by-step round execution (`Tour Suivant`) or automated simulation loop (`Lecture Auto` / `Pause`).
  - Real-time statistics: Round counter, alive/dead count, active CH count, and average network residual energy.
  - Event log console tracking elections, cycles, and packet transmissions.

---

## 📐 Mathematical & Radio Model

### 1. Energy Dissipation Model
For transmitting a packet of $k$ bits over distance $d$:

$$
E_{Tx}(k, d) =
\begin{cases}
k \cdot E_{elec} + k \cdot \epsilon_{fs} \cdot d^2 & \text{if } d < d_0 \\
k \cdot E_{elec} + k \cdot \epsilon_{mp} \cdot d^4 & \text{if } d \ge d_0
\end{cases}
$$

For receiving a packet of $k$ bits:

$$
E_{Rx}(k) = k \cdot E_{elec}
$$

Data aggregation cost for a Cluster Head receiving $m$ member packets:

$$
E_{agg} = m \cdot k \cdot E_{DA}
$$

#### Model Parameters (`LeachConstants.cs`):
| Parameter | Description | Value |
| :--- | :--- | :--- |
| $E_{elec}$ | Radio electronics energy | $50\text{ nJ/bit}$ |
| $\epsilon_{fs}$ | Free space amplifier factor | $10\text{ pJ/bit/m}^2$ |
| $\epsilon_{mp}$ | Multipath amplifier factor | $0.0013\text{ pJ/bit/m}^4$ |
| $E_{DA}$ | Data aggregation energy | $5\text{ nJ/bit}$ |
| $d_0$ | Distance threshold ($\sqrt{\epsilon_{fs}/\epsilon_{mp}}$) | $\approx 87.7\text{ m}$ |
| $k$ | Standard data packet size | $4000\text{ bits}$ ($500\text{ bytes}$) |
| $P$ | Default desired percentage of Cluster Heads | $5\% \ (0.05)$ |

---

### 2. Cluster Head Election Formulas

At each round $r$, an eligible node $n \in G$ (nodes that have not been CH in the current cycle of $1/P$ rounds) picks a random number between $0$ and $1$. If the number is strictly lower than the threshold $T(n)$, the node is elected Cluster Head:

#### Formula 1: Classical LEACH
$$
T(n) = \frac{P}{1 - P \times \left(r \pmod{\frac{1}{P}}\right)}, \quad \forall n \in G
$$

#### Formula 2: Energy-Aware
Weights the election threshold by the sensor's remaining battery percentage:
$$
T(n) = \frac{P}{1 - P \times \left(r \pmod{\frac{1}{P}}\right)} \times \frac{E_{current}}{E_{max}}
$$

#### Formula 3: Energy & Starvation Compensation
Factors in consecutive rounds without being elected ($r_s$):
$$
T(n) = \frac{P}{1 - P \times \left(r \pmod{\frac{1}{P}}\right)} \times \left[ \frac{E_{current}}{E_{max}} + \left(r_s \operatorname{div} \frac{1}{P}\right) \times \left(1 - \frac{E_{current}}{E_{max}}\right) \right]
$$

---

## 📂 Project Structure

```
├── Form1.cs                 # WinForms UI events, user controls, and GDI+ canvas rendering
├── Form1.Designer.cs        # UI component declarations & layout configuration
├── Network.cs               # Simulation engine, clustering algorithms, and round cycles
├── Node.cs                  # Node class (coordinates, battery states, cluster membership)
├── Message.cs               # Packet abstraction & energy calculation (Tx / Rx / Aggregation)
├── LeachConstants.cs        # Radio dissipation constants and default simulation parameters
├── Program.cs               # Application entry point
├── leach simulation.csproj  # .NET project configuration
└── README.md                # Project documentation
```

---

## 🚀 Getting Started

### Prerequisites
- **Operating System**: Windows 10 / 11
- **Runtime / SDK**: [.NET SDK](https://dotnet.microsoft.com/download) (.NET 8.0, 9.0, or 10.0)
- **IDE** (optional): Visual Studio 2022+ or VS Code with C# Dev Kit

### Installation & Run

1. **Clone the repository**:
   ```bash
   git clone https://github.com/<your-username>/<your-repo-name>.git
   cd <your-repo-name>
   ```

2. **Build and Run via CLI**:
   ```bash
   dotnet build
   dotnet run
   ```

3. **Or Open in Visual Studio**:
   - Double-click `leach simulation.csproj` or `leach simulation.slnx`
   - Press **F5** or click **Start**

---

## 🎮 How to Use

1. **Set Parameters**:
   - **Nodes**: Number of sensor nodes to deploy (e.g. `100`).
   - **Initial Energy (J)**: Starting energy per node (e.g. `1.0`).
   - **Range (m)**: Communication range radius (e.g. `130`).
   - **P (CH %)**: Desired CH probability (e.g. `0.05`).
2. **Select Threshold Formula**:
   - Choose between Formula 1 (Classical), Formula 2 (Residual Energy), or Formula 3 (Energy + Non-elected factor).
3. **Generate Network**:
   - Click `Generer Reseau` to deploy nodes randomly across the canvas.
4. **Run Simulation**:
   - Click `Tour Suivant` to advance one round at a time.
   - Click `Lecture Auto` to let the simulation run automatically with real-time visual updates.
   - Observe battery depletion, node deaths, and cluster formations in the statistics pane and event log.

---

## 📜 License

This project is open-source and available under the [MIT License](LICENSE).

