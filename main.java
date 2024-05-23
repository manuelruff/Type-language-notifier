import java.awt.event.*;
import java.io.File;
import java.util.concurrent.TimeUnit;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Clip;
import javax.swing.*;

class KeyPressListener {
    // Sound file locations
    private static final File hebrew = new File("sounds\\Hebrew.wav");
    private static final File english = new File("sounds\\English.wav");
    // Record the time of the last key press
    private static long lastKeyPressTime = 0;
    // Define the delay in milliseconds (30 seconds)
    private static final long delay = TimeUnit.SECONDS.toMillis(30);

    public static void main(String[] args) {
        System.out.println("Hello World");

        // Create and show GUI
        SwingUtilities.invokeLater(() -> {
            createAndShowGUI();
        });
    }

    /**
     * Plays a sound from the specified file location.
     *
     * @param language the file location of the sound to be played
     */
    public static void playSound(File language) {
        try {
            AudioInputStream audioInputStream = AudioSystem.getAudioInputStream(language);
            Clip clip = AudioSystem.getClip();
            clip.open(audioInputStream);
            clip.start();
        } catch (Exception e) {
            System.out.println("Error with playing sound.");
        }
    }

    private static void createAndShowGUI() {
        JFrame frame = new JFrame("Key Press Listener");
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        JTextField textField = new JTextField(20);
        textField.addKeyListener(new MyKeyListener());

        frame.getContentPane().add(textField);
        frame.pack();
        frame.setVisible(true);
    }

    static class MyKeyListener implements KeyListener {
        public void keyPressed(KeyEvent e) {
            long currentTime = System.currentTimeMillis();
            // Check if enough time has passed since the last key press
            if (currentTime - lastKeyPressTime >= delay) {
                playSound(hebrew);
                // Update the last key press time
                lastKeyPressTime = currentTime;
            }
        }

        public void keyReleased(KeyEvent e) {
            // Not needed for this example
        }

        public void keyTyped(KeyEvent e) {
            // Not needed for this example
        }
    }
}
